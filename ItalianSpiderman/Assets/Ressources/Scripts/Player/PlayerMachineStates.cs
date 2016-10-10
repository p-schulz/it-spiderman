using UnityEngine;
using System.Collections;

public partial class PlayerMachine : SuperStateMachine {

    /// <summary>
    /// Italian Spiderman Idling
    /// </summary>

    // ISpid is starting to stand, nothing to do here -> idel_superupdate
    void Idle_EnterState() {}

    // ISpid is standing around (Idle)
    void Idle_SuperUpdate() {
        // ISpid is standing on a too steep slope
        if (IsSliding() && moveDirection == Vector3.zero) {
            currentState = PlayerStates.Slide;
            return;
        }

        // ISpid without ground -> falling
        if (!MaintainingGround()) {
            currentState = PlayerStates.Fall;
            return;
        }

        // ------------------------------------------------------------------
        // Jumping, Striking and Kicking are prevented by falldamage -> check

        // ISpid is jumping
        if (!isTakingFallDamage && input.Current.JumpDown) {
            currentJumpProfile = ResolveJump();
            currentState = PlayerStates.Jump;
            return;
        }

        // ISpid is striking
        if (!isTakingFallDamage && input.Current.StrikeDown) {
            currentState = PlayerStates.Strike;
            return;
        }
        
        // ISpid is kicking
        if (!isTakingFallDamage && input.Current.KickDown) {
            currentState = PlayerStates.Kick;
            return;
        }

        // Ispid is moving
        if (input.Current.MoveInput != Vector3.zero) {
            currentState = PlayerStates.Run;
            return;
        }
        // probably fade out on running
        else {
            moveSpeed = Mathf.MoveTowards(moveSpeed, 0, 60.0f * controller.deltaTime);
            moveDirection = Math3d.SetVectorLength(moveDirection, Mathf.Abs(moveSpeed));
        }

        // recheck for groud because running fade may lead to falling
        if (!MaintainingGround()) {
            currentState = PlayerStates.Fall;
            return;
        }

        // standing and no standing animation or landing or getting up
        if (!anim.IsPlaying("weight_shift") && !anim.IsPlaying("land") && !anim.IsPlaying("getting_up_1")) {
            // start wheight shifting animation
            anim.CrossFade("weight_shift", 0.3f);
        }
    }

    /// <summary>
    /// Italian Spiderman Running
    /// </summary>

    // running direction
    private Vector3 cachedDirection;

    // ISpid begins to run
    void Run_EnterState() {
        // init direction, animation and sound
        cachedDirection = lookDirection;
        anim.CrossFade("running_inPlace", 0.15f);
        sound.StartFootsteps(anim["running_inPlace"].length / 1.7f / 2f, 0.185f);
    }

    // ISpid is running check for input
    void Run_SuperUpdate() {
        lookDirection = cachedDirection;

        RunSmokeEffect.enableEmission = false;

        // ISpid is jumping
        if (!isTakingFallDamage && input.Current.JumpDown) {
            currentJumpProfile = ResolveJump();
            currentState = PlayerStates.Jump;
            return;
        }

        if (!MaintainingGround()) {
            currentState = PlayerStates.Fall;
            return;
        }

        // ISpid is starting to kick from running
        if (!isTakingFallDamage && input.Current.KickDown) {
            // ISpid was running fast
            if (moveSpeed > runSpeed * 0.9f)
            {
                transform.position += controller.up * 0.3f;
                verticalMoveSpeed = 6.0f;
                currentState = PlayerStates.Kick;

                // TODO implement flying kick state and activate it here
                // -------------------------------------------------------------------------------------------------------------------------------------

                return;
            }
            // ISpid was not running too fast
            else
            {
                currentState = PlayerStates.Kick;
                return;
        }
    }

        // ISpid is starting to strike from running
        if (!isTakingFallDamage && input.Current.StrikeDown) {
            // ISpid was running fast 
            if (moveSpeed > runSpeed * 0.9f) {
                transform.position += controller.up * 0.3f;
                verticalMoveSpeed = 6.0f;
                sound.PlayGroundDive();
                currentState = PlayerStates.Dive;
                return;
            }
            // ISpid was not running too fast
            else {
                currentState = PlayerStates.Strike;
                return;
            }
        }

        if (IsSliding()) {
            if (Vector3.Angle(Math3d.ProjectVectorOnPlane(controller.currentGround.Hit.normal, lookDirection), SlopeDirection()) < 90.0f)
            {
                currentState = PlayerStates.Slide;
                return;
            }
        }

        Vector3 wallCollisionNormal = Vector3.zero;
        float wallCollisionAngle = 0;

        if (input.Current.MoveInput != Vector3.zero)
        {
            if (Vector3.Angle(input.Current.MoveInput, lookDirection) > 110)
            {
                if (IsSliding())
                {
                    currentState = PlayerStates.Slide;
                    return;
                }
                else
                {
                    if (moveSpeed > runSpeed * 0.56f)
                    {
                        currentState = PlayerStates.Stop;
                        return;
                    }
                    else if (moveSpeed == 0)
                    {
                        lookDirection = input.Current.MoveInput;
                    }
                }
            }

            Vector3 moveDirectionProjected = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);

            RotateLookDirection(input.Current.MoveInput, turnSpeed);

            float targetSpeed = isTakingFallDamage ? runSpeed * 0.3f : runSpeed;

            if (!IsSliding())
            {
                SuperCollision col;

                if (HasWallCollided(out col) && Vector3.Angle(moveDirectionProjected, -col.normal) < 65.0f)
                {
                    moveSpeed = runSpeed * 0.2f;
                    wallCollisionNormal = col.normal;
                    wallCollisionAngle = Vector3.Angle(moveDirectionProjected, -col.normal);
                }
                else
                {
                    float acceleration = targetSpeed * input.Current.MoveMagnitude >= moveSpeed ? SuperMath.BoundedInterpolation(new float[] { 1.3f, 3f, 6f }, new float[] { 16, 12, 8, 6 }, moveSpeed) : 16;
                    moveSpeed = Mathf.MoveTowards(moveSpeed, targetSpeed * input.Current.MoveMagnitude, acceleration * controller.deltaTime);
                }
            }
            else
            {
                float t = Mathf.InverseLerp(20.0f, 60.0f, GroundAngle());
                float deccelerationModifier = Mathf.Lerp(1.0f, 3.0f, t);
                float decceleration = SuperMath.BoundedInterpolation(new float[] { 1.3f, 3f, 6f }, new float[] { 1 * deccelerationModifier, 4 * deccelerationModifier, 7 * deccelerationModifier, 15 * deccelerationModifier}, moveSpeed);
                moveSpeed = Mathf.MoveTowards(moveSpeed, 0, decceleration * controller.deltaTime);

                if (moveSpeed == 0)
                {
                    currentState = PlayerStates.Slide;
                    return;
                }
            }

            Vector3 f = Vector3.Cross(controller.currentGround.Hit.normal, Vector3.Cross(lookDirection, Vector3.up));
            moveDirection = Math3d.SetVectorLength(f, moveSpeed);
        }
        else
        {
            if (IsSliding())
            {
                currentState = PlayerStates.Slide;
                return;
            }
            else
            {
                if (moveSpeed > runSpeed * 0.66f)
                {
                    currentState = PlayerStates.Stop;
                    return;
                }
                else
                {
                    currentState = PlayerStates.Idle;
                    return;
                }
            }
        }

        Vector3 previousLookDirection = cachedDirection;
        cachedDirection = lookDirection;

        float artRotationSpeed = 6.0f;

        if (wallCollisionNormal != Vector3.zero)
        {
            if (wallCollisionAngle < 20.0f)
            {
                if (!anim.IsPlaying("pushing"))
                {
                    anim.CrossFade("pushing", 0.05f);
                    sound.EndFootsteps();
                    sound.StartFootsteps(anim["walking_inPlace"].length / 2f, 0.135f);
                }

                chestBendAngle = 0;

                chestTwistAngle = Mathf.MoveTowards(chestTwistAngle, 0, 200.0f * controller.deltaTime);
            }
            else
            {
                if (Vector3.Angle(Math3d.ProjectVectorOnPlane(controller.up, moveDirection), Vector3.Cross(wallCollisionNormal, controller.up)) > 90)
                {
                    if (!anim.IsPlaying("crouch_left"))
                    {
                        anim.CrossFade("crouch_left", 0.05f);
                        sound.EndFootsteps();
                    }

                    chestBendAngle = 0;

                    chestTwistAngle = Mathf.MoveTowards(chestTwistAngle, 0, 200.0f * controller.deltaTime);
                }
                else
                {
                    if (!anim.IsPlaying("crouch_right"))
                    {
                        anim.CrossFade("crouch_right", 0.05f);
                        sound.EndFootsteps();
                    }

                    chestBendAngle = 0;

                    chestTwistAngle = Mathf.MoveTowards(chestTwistAngle, 0, 200.0f * controller.deltaTime);
                }
            }

            artUpDirection = Vector3.RotateTowards(artUpDirection, controller.up, artRotationSpeed * controller.deltaTime, 0);

            lookDirection = -wallCollisionNormal;
        }
        else
        {
            if (input.Current.MoveMagnitude > 0.5)
            {
                if (!anim.IsPlaying("running_inPlace"))
                {
                    anim.CrossFade("running_inPlace", 0.15f);
                    sound.EndFootsteps();
                    sound.StartFootsteps(anim["running_inPlace"].length / 1.7f / 2f, 0.185f);
                }

                if (!SuperMath.Timer(timeEnteredState, 1.0f))
                {
                    RunSmokeEffect.enableEmission = true;
                }

                if (GroundAngle() > 15.0f)
                {
                    var projectedSlopeDirection = Math3d.ProjectVectorOnPlane(controller.up, SlopeDirection());
                    var dot = Vector3.Dot(projectedSlopeDirection, lookDirection);

                    dot = Mathf.Clamp(dot * 2, -1, 1);

                    float artAngle = Mathf.Lerp(0, GroundAngle(), Mathf.Abs(dot));

                    Vector3 right = Vector3.Cross(controller.up, lookDirection);

                    artUpDirection = Vector3.RotateTowards(artUpDirection, (Quaternion.AngleAxis(artAngle * Mathf.Sign(dot), right) * controller.up).normalized, artRotationSpeed * controller.deltaTime, 0);
                }
                else
                {
                    artUpDirection = Vector3.RotateTowards(artUpDirection, controller.up, artRotationSpeed * controller.deltaTime, 0);
                }

                float lerpValue = Mathf.InverseLerp(runSpeed * 0.5f, runSpeed * 0.9f, moveSpeed);
                chestBendAngle = Mathf.Lerp(0, -18, lerpValue);

                Vector3 previousRight = Vector3.Cross(controller.up, previousLookDirection);

                bool turningRight = Vector3.Angle(previousLookDirection, previousRight) > Vector3.Angle(lookDirection, previousRight);

                float lookAngleDifference = Vector3.Angle(previousLookDirection, lookDirection) * controller.deltaTime;

                if (lookAngleDifference > 0.05f)
                    chestTwistAngle = turningRight ? Mathf.MoveTowards(chestTwistAngle, -18.0f, 200.0f * controller.deltaTime) : Mathf.MoveTowards(chestTwistAngle, 18.0f, 200.0f * controller.deltaTime);
                else
                    chestTwistAngle = Mathf.MoveTowards(chestTwistAngle, 0, 200.0f * controller.deltaTime);
                
            }
            else if (input.Current.MoveMagnitude > 0.25)
            {
                if (!anim.IsPlaying("walking_inPlace"))
                {
                    anim.CrossFade("walking_inPlace", 0.15f);
                    sound.EndFootsteps();
                    sound.StartFootsteps(anim["walking_inPlace"].length / 2f, 0.125f);
                }

                artUpDirection = Vector3.RotateTowards(artUpDirection, controller.up, artRotationSpeed * controller.deltaTime, 0);

                chestBendAngle = 0;

                chestTwistAngle = Mathf.MoveTowards(chestTwistAngle, 0, 200.0f * controller.deltaTime);
            }
            else
            {
                if (!anim.IsPlaying("weight_shift"))
                {
                    anim.CrossFade("weight_shift", 0.15f);
                    sound.EndFootsteps();
                    sound.StartFootsteps(anim["weight_shift"].length / 2f, 0.95f);
                }

                artUpDirection = Vector3.RotateTowards(artUpDirection, controller.up, artRotationSpeed * controller.deltaTime, 0);

                chestBendAngle = 0;

                chestTwistAngle = Mathf.MoveTowards(chestTwistAngle, 0, 200.0f * controller.deltaTime);
            }
        }
    }

    // ISpid stopping to run
    void Run_ExitState() {
        chestBendAngle = 0;
        chestTwistAngle = 0;
        artUpDirection = controller.up;

        RunSmokeEffect.enableEmission = false;

        sound.EndFootsteps();
    }

    /// <summary>
    /// Italian Spiderman Stopping
    /// </summary>

    void Stop_EnterState() {
        //anim.CrossFade("stop", 0.1f);

        sound.PlaySkid();

        RunSmokeEffect.enableEmission = true;
    }

    void Stop_SuperUpdate() {
        if (!MaintainingGround())
        {
            currentState = PlayerStates.Fall;
            return;
        }

        if (Vector3.Angle(input.Current.MoveInput, lookDirection) > 110 && input.Current.JumpDown)
        {
            lookDirection *= -1;
            currentJumpProfile = jumpSideFlip;
            currentState = PlayerStates.Jump;
            return;
        }

        moveSpeed = Mathf.MoveTowards(moveSpeed, 0, 60.0f * controller.deltaTime);
        moveDirection = Math3d.SetVectorLength(moveDirection, moveSpeed);

        if (input.Current.MoveInput != Vector3.zero)
        {
            if (Vector3.Angle(input.Current.MoveInput, lookDirection) < 110)
            {
                currentState = PlayerStates.Run;
                return;
            }
            else
            {
                if (moveSpeed == 0)
                {
                    lookDirection *= -1;
                    currentState = PlayerStates.Turn;
                    return;
                }
            }
        }

        if (moveSpeed == 0)
        {
            currentState = PlayerStates.Idle;
            return;
        }
    }

    void Stop_ExitState() {
        RunSmokeEffect.enableEmission = false;
    }

    /// <summary>
    /// Italian Spiderman Turning
    /// </summary>

    void Turn_EnterState() {
        anim.Play("running_inPlace");
    }

    void Turn_SuperUpdate() {
        if (!MaintainingGround()) {
            currentState = PlayerStates.Fall;
            return;
        }

        if (input.Current.JumpDown) {
            currentJumpProfile = jumpSideFlip;
            currentState = PlayerStates.Jump;
            return;
        }

        if (input.Current.MoveInput != Vector3.zero) {
            Vector3 f = Vector3.Cross(controller.currentGround.Hit.normal, Vector3.Cross(lookDirection, Vector3.up));

            RotateLookDirection(input.Current.MoveInput, turnSpeed);

            float acceleration = 20.0f * controller.deltaTime;

            moveSpeed = Mathf.MoveTowards(moveSpeed, runSpeed, acceleration);

            moveDirection = Math3d.SetVectorLength(f, moveSpeed);
        }
        else {
            moveSpeed = Mathf.MoveTowards(moveSpeed, 0, 15.0f * controller.deltaTime);
            moveDirection = Math3d.SetVectorLength(moveDirection, moveSpeed);
        }

        if (Timer(timeEnteredState, anim["running_inPlace"].length)) {
            currentState = PlayerStates.Idle;
        }
    }

    /// <summary>
    /// Italian Spiderman Sliding
    /// </summary>

    private Vector3 targetDirection;

    void Slide_EnterState() {
        targetDirection = SlopeDirection();

        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);

        moveDirection = Math3d.ProjectVectorOnPlane(GroundNormal(), planarMoveDirection);
        moveDirection = Math3d.SetVectorLength(moveDirection, Mathf.Abs(moveSpeed));

        RunSmokeEffect.enableEmission = true;

        anim.Play("roll");

        sound.PlaySlide();
    }

    void Slide_SuperUpdate() {
        if (!MaintainingGround()) {
            currentState = PlayerStates.Fall;
            return;
        }

        if (!IsContinueSliding() && (input.Current.JumpDown || input.Current.StrikeDown)) {
            currentState = PlayerStates.SFlip;
            return;
        }

        if (!IsContinueSliding() && (input.Current.JumpDown || input.Current.KickDown))
        {
            currentState = PlayerStates.SFlip;
            return;
        }

        if (!goldPlayer) {
            BodySlam();
        }
        else {
            GoldBodySlam();
        }

        SuperCollision col;

        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);

        if (HasWallCollided(out col)) {
            if (Vector3.Angle(-planarMoveDirection.normalized, col.normal) < 75.0f)
            {
                moveSpeed = -2.0f;
                lookDirection = Vector3.Reflect(-Math3d.ProjectVectorOnPlane(controller.up, moveDirection), col.normal).normalized;

                Instantiate(WallHitStarEffect, col.point, Quaternion.LookRotation(col.normal));

                sound.PlayHeavyKnockback();
                currentState = PlayerStates.Knockback;
                return;
            }
        }

        Vector3 planarSlopeDirection = Math3d.ProjectVectorOnPlane(controller.up, SlopeDirection());

        Vector3 projectedInput = Math3d.ProjectVectorOnPlane(GroundNormal(), input.Current.MoveInput).normalized;

        bool movingDownSlope = Vector3.Angle(planarMoveDirection, planarSlopeDirection) < 90;
        bool feetFirst = Vector3.Angle(lookDirection, planarSlopeDirection) > 140;

        moveDirection = Math3d.ProjectVectorOnPlane(GroundNormal(), moveDirection);
        moveDirection = Math3d.SetVectorLength(moveDirection, Mathf.Abs(moveSpeed));

        if (IsContinueSliding())
        {
            if (movingDownSlope && input.Current.MoveInput != Vector3.zero && Vector3.Angle(input.Current.MoveInput, planarSlopeDirection) < 110.0f && Vector3.Angle(planarMoveDirection, planarSlopeDirection) < 70.0f)
            {
                targetDirection = Vector3.RotateTowards(targetDirection, projectedInput, turnSpeed * 0.4f * controller.deltaTime, 0);
                targetDirection = SuperMath.ClampAngleOnPlane(SlopeDirection(), targetDirection, 70.0f, GroundNormal());
            }
            else
            {
                targetDirection = SlopeDirection();
            }

            Vector3 horizontalMovement = Math3d.ProjectVectorOnPlane(targetDirection, moveDirection);
            Vector3 slopingMovement = moveDirection - horizontalMovement;

            horizontalMovement = Vector3.MoveTowards(horizontalMovement, Vector3.zero, 5.0f * controller.deltaTime);
            slopingMovement = Vector3.MoveTowards(slopingMovement, targetDirection * maxSlideSpeed, 20.0f * controller.deltaTime);

            moveDirection = horizontalMovement + slopingMovement;
            moveSpeed = moveDirection.magnitude;

            if (Vector3.Angle(lookDirection, planarSlopeDirection) > 90 && Vector3.Angle(lookDirection, planarMoveDirection) > 90)
            {
                moveSpeed *= -1;
            }

            if (movingDownSlope)
            {
                if (feetFirst)
                {
                    RotateLookDirection(-targetDirection, turnSpeed * 0.05f);
                }
                else
                {
                    RotateLookDirection(targetDirection, turnSpeed * 0.15f);
                }
            }
        }
        else
        {
            moveSpeed = Mathf.MoveTowards(moveSpeed, 0, 15 * controller.deltaTime);
            moveDirection = Math3d.SetVectorLength(moveDirection, Mathf.Abs(moveSpeed));

            if (input.Current.MoveInput != Vector3.zero)
            {
                moveDirection = Vector3.RotateTowards(moveDirection, projectedInput, turnSpeed * 0.1f * controller.deltaTime, 0);
            }

            if (moveSpeed < 0)
            {
                RotateLookDirection(-planarMoveDirection, turnSpeed * 0.1f);
            }
            else
            {
                RotateLookDirection(planarMoveDirection, turnSpeed * 0.1f);
            }

        }

        if (moveSpeed == 0)
        {
            currentState = PlayerStates.SlideRecover;
            return;
        }

        if (Mathf.Abs(moveSpeed) < 4.0f)
        {
            RunSmokeEffect.enableEmission = false;
        }
        else
        {
            RunSmokeEffect.enableEmission = true;
        }

        artUpDirection = GroundNormal();
    }

    void Slide_ExitState() {
        sound.Stop();

        artUpDirection = controller.up;

        RunSmokeEffect.enableEmission = false;
    }

    /// <summary>
    /// Italian Spiderman Jumping
    /// </summary>

    private bool slopeJumping;
    private bool bouncing;
    private bool hasStruck;
    private Vector3 jumpPeak;

    // ISpid starting to jump
    void Jump_EnterState() {
        controller.DisableClamping();
        controller.DisableSlopeLimit();

        bouncing = false;

        hasStruck = false;

        // Is ISpid jumping from slope
        if (currentJumpProfile != jumpTriple) {
            slopeJumping = IsSliding();
        }
        else {
            slopeJumping = false;
        }

        // ISpid is slowed by jump
        if (slopeJumping && moveSpeed > runSpeed * 0.5f) {
            moveSpeed = runSpeed * 0.5f;
        }

        // calc vertical movement for jump
        verticalMoveSpeed = CalculateJumpSpeed(currentJumpProfile.JumpHeight, currentJumpProfile.Gravity);

        // TODO delete if (wtf)
        if (currentJumpProfile.InitialForwardVelocity != 0) {
            moveSpeed += currentJumpProfile.InitialForwardVelocity;
        }

        moveDirection += lookDirection * moveSpeed + controller.up * verticalMoveSpeed;

        anim.CrossFade(currentJumpProfile.Animation, currentJumpProfile.CrossFadeTime);

        jumpPeak = transform.position;

        PlayJumpSound();
    }

    // ISpid is jumping
    void Jump_SuperUpdate() {

        // ISpid is kicking midair
        if (input.Current.KickDown) {
            if (currentJumpProfile.CanKick && (moveSpeed <= 7.0f || verticalMoveSpeed > CalculateJumpSpeed(currentJumpProfile.JumpHeight, currentJumpProfile.Gravity) * 0.85f) && !slopeJumping) {
                currentJumpProfile = jumpKick;
                currentState = PlayerStates.Jump;
                return;
            }
            else if (currentJumpProfile.CanKick && ((moveSpeed > 7.0f && moveSpeed < 9.0f) || verticalMoveSpeed > CalculateJumpSpeed(currentJumpProfile.JumpHeight, currentJumpProfile.Gravity) * 0.85f) && !slopeJumping) {
                currentJumpProfile = jumpFastKick;
                currentState = PlayerStates.Jump;
                return;
            }
            else if (currentJumpProfile.CanDive) {
                sound.PlayDive();
                currentState = PlayerStates.Dive;
                return;
            }
        }

        // ISpid is striking midair
        if (input.Current.StrikeDown) {
            if (currentJumpProfile.CanKick && (moveSpeed <= 7.0f || verticalMoveSpeed > CalculateJumpSpeed(currentJumpProfile.JumpHeight, currentJumpProfile.Gravity) * 0.85f) && !slopeJumping) {
                currentJumpProfile = jumpStrike;
                currentState = PlayerStates.Jump;
                return;
            }
            else if (currentJumpProfile.CanKick && ((moveSpeed > 7.0f && moveSpeed < 9.0f) || verticalMoveSpeed > CalculateJumpSpeed(currentJumpProfile.JumpHeight, currentJumpProfile.Gravity) * 0.85f) && !slopeJumping) {
                currentJumpProfile = jumpFastStrike;
                currentState = PlayerStates.Jump;
                return;
            }
            else if (currentJumpProfile.CanDive) {
                sound.PlayDive();
                currentState = PlayerStates.Dive;
                return;
            }
        }

        // ISpid is doing a jump kick
        if (currentJumpProfile == jumpKick) {

            if (!hasStruck && SuperMath.Timer(timeEnteredState, 0.05f) && !SuperMath.Timer(timeEnteredState, 1.2f)) {
                GameObject struckObject;

                if (Strike(out struckObject, GetKickOrigin(), GetKickOffset(), controller.radius * 1.5f)) {

                    TriggerableObject trigger = struckObject.GetComponent<TriggerableObject>();
                    EnemyMachine machine = struckObject.GetComponent<EnemyMachine>();                    

                    if (machine != null) {
                        moveSpeed = -5.0f;

                        //if (goldPlayer) {
                        //    machine.GetStruck(lookDirection, 10.0f, 15.0f);
                        //    machine.MakeGold();
                        //}
                        //else {
                            machine.GetStruck(lookDirection, 10.0f, 15.0f, 0.3f);
                        //}

                        Instantiate(EnemyHitEffect, GetKickPosition(), Quaternion.LookRotation(-lookDirection));
                    }
                    else if (trigger != null) {
                        moveSpeed = -12.0f;

                        trigger.Strike();

                        Instantiate(EnemyHitEffect, GetKickPosition(), Quaternion.LookRotation(-lookDirection));
                    }
                    else {
                        moveSpeed = -12.0f;

                        Instantiate(EnemyHitEffect, GetKickPosition(), Quaternion.LookRotation(-lookDirection));
                    }
                    sound.PlayImpact();
                    hasStruck = true;

                    return;
                }
            }
        }

        // ISpid is doing a jump strike
        if (currentJumpProfile == jumpStrike) {

            if (!hasStruck && SuperMath.Timer(timeEnteredState, 0.05f) && !SuperMath.Timer(timeEnteredState, 1.2f)) {
                GameObject struckObject;

                if (Strike(out struckObject, GetKickOrigin(), GetKickOffset(), controller.radius * 1.5f)) {

                    TriggerableObject trigger = struckObject.GetComponent<TriggerableObject>();
                    EnemyMachine machine = struckObject.GetComponent<EnemyMachine>();

                    if (machine != null) {
                        moveSpeed = -5.0f;

                        //if (goldPlayer) {
                        //    machine.GetStruck(lookDirection, 10.0f, 15.0f);
                        //    machine.MakeGold();
                        //}
                        //else {
                        machine.GetStruck(lookDirection, 10.0f, 15.0f, 0.3f);
                        //}

                        Instantiate(EnemyHitEffect, GetKickPosition(), Quaternion.LookRotation(-lookDirection));
                    }
                    else if (trigger != null) {
                        moveSpeed = -12.0f;

                        trigger.Strike();

                        Instantiate(EnemyHitEffect, GetKickPosition(), Quaternion.LookRotation(-lookDirection));
                    }
                    else {
                        moveSpeed = -12.0f;

                        Instantiate(EnemyHitEffect, GetKickPosition(), Quaternion.LookRotation(-lookDirection));
                    }
                    sound.PlayImpact();
                    hasStruck = true;

                    return;
                }
            }
        }

        // ISpid reached the jumping peak and is falling
        if (verticalMoveSpeed < 0) {
            // set new jumping peek if current position is above the current jumping peak level
            // how is this going to happen? - no idea (first time this case is called)
            if (SuperMath.PointAbovePlane(controller.up, jumpPeak, transform.position)) {
                jumpPeak = transform.position;
            }

            GameObject struckObject;

            if (FootStrike(out struckObject)) {
                VillainMachine Villain = struckObject.GetComponent<VillainMachine>();
                if (Villain) {
                    Villain.KillEnemy();
                }
                verticalMoveSpeed = 10.0f;
                bouncing = true;
            }

            if (AcquiringGround()) {
                currentState = PlayerStates.Land;
                return;
            }

            Vector3 ledgePosition;
            GameObject grabbedLedge;

            if (CanGrabLedge(out ledgePosition, out grabbedLedge)) {
                GrabLedge(ledgePosition);
                controller.currentlyClampedTo = grabbedLedge.transform;
                currentState = PlayerStates.Hang;
                return;
            }

            // If falling, continue falling
            if (currentJumpProfile.FallAnimation != null && !anim.IsPlaying(currentJumpProfile.FallAnimation)) {
                anim.CrossFade(currentJumpProfile.FallAnimation, 0.2f);
            }
        }
        // ISpid jumping and gaining height
        else {
            SuperCollision c;

            if (HasHeadCollided(out c)) {
                verticalMoveSpeed = 0;

                var trigger = c.gameObject.GetComponent<TriggerableObject>();

                if (trigger != null) {
                    trigger.BottomHit();
                }
            }

            if (currentJumpProfile.CanControlHeight && !bouncing && !input.Current.Jump) {
                verticalMoveSpeed = Mathf.MoveTowards(verticalMoveSpeed, 0, 160.0f * controller.deltaTime);
            }
        }

        SuperCollision col;
        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);

        // ISpid jumped against a wall
        if (HasWallCollided(out col)) {
            if (WallCollisionAngle(-col.normal, moveDirection) < 40.0f) {
                Vector3 r = Vector3.Cross(controller.up, col.normal);

                Vector3 relativeMagnitude = Math3d.ProjectVectorOnPlane(r, planarMoveDirection);

                if (relativeMagnitude.magnitude > 5.0f) {
                    Vector3 o = controller.OffsetPosition(controller.head.Offset) + (controller.up * controller.radius);

                    RaycastHit hit;

                    if (col.gameObject.GetComponent<Collider>().Raycast(new Ray(o, Math3d.ProjectVectorOnPlane(controller.up, col.point - o)), out hit, controller.radius * 1.5f))
                    {
                        if (Vector3.Angle(-planarMoveDirection.normalized, col.normal) < 40.0f && Vector3.Angle(controller.up, hit.normal) > 70.0f)
                        {
                            lookDirection = Vector3.Reflect(lookDirection, r);
                            moveSpeed = -2.0f;
                            verticalMoveSpeed = 0;

                            wallHitNormal = col.normal;
                            wallHitTime = Time.time;

                            sound.PlayWallHit();

                            currentState = PlayerStates.Fall;
                            return;
                        }
                    }
                    else {
                        moveSpeed = 0;
                    }
                }
                else {
                    moveSpeed = 0;
                }
            }
        }

        Vector3 horizontalMovement = Vector3.zero;

        // ISpids movement in air
        if (!slopeJumping) {
            if (input.Current.MoveInput != Vector3.zero) {
                Vector3 relativeMoveInput = Math3d.ProjectVectorOnPlane(Vector3.Cross(controller.up, lookDirection), input.Current.MoveInput);
                float relativeAcceleration = Mathf.Clamp(relativeMoveInput.magnitude, -0.7f, 0.7f) * 1/0.7f;

                float targetMovement;

                if (Vector3.Angle(lookDirection, relativeMoveInput) < 90) {
                    targetMovement = relativeAcceleration * Mathf.Max(runSpeed, currentJumpProfile == jumpLong ? runSpeed + currentJumpProfile.InitialForwardVelocity : runSpeed);
                }
                else {
                    targetMovement = relativeAcceleration * -runSpeed * 0.5f;
                }

                moveSpeed = Mathf.MoveTowards(moveSpeed, targetMovement, 14.0f * controller.deltaTime);
                horizontalMovement = Math3d.ProjectVectorOnPlane(lookDirection, input.Current.MoveInput) * 220.0f * controller.deltaTime;

            }
            else {
                moveSpeed = Mathf.MoveTowards(moveSpeed, 0, 8.0f * controller.deltaTime);
            }
        }

        verticalMoveSpeed = Mathf.MoveTowards(verticalMoveSpeed, -currentJumpProfile.MaximumGravity, currentJumpProfile.Gravity * controller.deltaTime);
        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed) + controller.up * verticalMoveSpeed + horizontalMovement;
    }

    void Jump_ExitState() {
        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed) + controller.up * verticalMoveSpeed;
    }

    /// <summary>
    /// Italian Spiderman Landing
    /// </summary>

    void Land_EnterState() {
        lastLandTime = Time.time;

        controller.EnableClamping();
        controller.EnableSlopeLimit();

        verticalMoveSpeed = 0;

        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed) + controller.up * verticalMoveSpeed;

        if (ShouldHaveFallDamage()) {
            if (FallDamage()) {
                currentState = PlayerStates.KnockbackForwards;
                moveSpeed = 3.0f;
                return;
            }
        }

        sound.PlayLand();
        anim.CrossFade("land", 0.5f);
    }

    void Land_SuperUpdate() {
        
        if (!MaintainingGround()) {
            currentState = PlayerStates.Fall;
            return;
        }

        if (!isTakingFallDamage && input.Current.JumpDown) {
            if (!IsSliding() && currentJumpProfile == jumpLong && input.Current.KickDown) {
                currentJumpProfile = jumpLong;
            }
            else {
                currentJumpProfile = ResolveJump();
            }

            sound.Stop();

            currentState = PlayerStates.Jump;
            return;
        }

        if (!isTakingFallDamage && input.Current.MoveInput != Vector3.zero)
        {
            if (Vector3.Angle(input.Current.MoveInput, lookDirection) > 110)
            {
                if (IsSliding())
                {
                    currentState = PlayerStates.Slide;
                    return;
                }
                else
                {
                    if (moveSpeed > runSpeed * 0.66f)
                    {
                        currentState = PlayerStates.Stop;
                        return;
                    }
                }
            }
            else
            {
                if (IsSliding())
                {
                    moveSpeed = Mathf.MoveTowards(moveSpeed, 0, 10.0f * controller.deltaTime);
                }
                else
                {
                    moveSpeed = Mathf.MoveTowards(moveSpeed, runSpeed, runSpeed / 1.36f * controller.deltaTime);
                }
            }
        }
        else
        {
            moveSpeed = Mathf.MoveTowards(moveSpeed, 0, 80.0f * controller.deltaTime);
        }

        Vector3 f = Vector3.Cross(controller.currentGround.Hit.normal, Vector3.Cross(lookDirection, Vector3.up));
        moveDirection = Math3d.SetVectorLength(f, moveSpeed);

        if (Time.time > timeEnteredState + 0.10f)
        {
            currentState = PlayerStates.Idle;
            return;
        }
    }

    /// <summary>
    /// Italian Spiderman Diving
    /// </summary>

    void Dive_EnterState() {
        anim["flying_inPlace"].speed *= 1.5f;
        anim.Play("flying_inPlace");

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        moveSpeed += 6.1f;

        jumpPeak = transform.position;
    }

    void Dive_SuperUpdate() {
        SuperCollision col;
        Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);

        if (SuperMath.PointAbovePlane(controller.up, jumpPeak, transform.position))
        {
            jumpPeak = transform.position;
        }

        if (HasWallCollided(out col))
        {
            if (Vector3.Angle(-planarMoveDirection.normalized, col.normal) < 75.0f)
            {
                moveSpeed = -2.0f;
                lookDirection = Vector3.Reflect(-lookDirection, col.normal);

                Instantiate(WallHitStarEffect, col.point, Quaternion.LookRotation(col.normal));

                sound.PlayHeavyKnockback();
                currentState = PlayerStates.AirKnockback;
                return;
            }
        }

        if (AcquiringGround())
        {
            verticalMoveSpeed = 0;

            if (ShouldHaveFallDamage())
            {
                if (FallDamage())
                {
                    moveSpeed = Mathf.Clamp(5.0f, 0, moveSpeed);
                    verticalMoveSpeed = 0;
                    currentState = PlayerStates.KnockbackForwards;
                    return;
                }
            }

            sound.PlayDiveLand();
            
            currentState = PlayerStates.Slide;
            return;
        }

        if (!goldPlayer)
        {
            BodySlam();
        }
        else
        {
            GoldBodySlam();
        }

        Vector3 right = Vector3.Cross(controller.up, lookDirection);
        Vector3 horizontalMovement = Vector3.zero;

        if (input.Current.MoveInput != Vector3.zero)
        {
            Vector3 relativeMoveInput = Math3d.ProjectVectorOnPlane(right, input.Current.MoveInput);

            float targetMovement;

            if (Vector3.Angle(lookDirection, relativeMoveInput) < 90)
            {
                targetMovement = relativeMoveInput.magnitude * (runSpeed + 6.1f);
            }
            else
            {
                targetMovement = relativeMoveInput.magnitude * runSpeed * 0.5f;
            }

            moveSpeed = Mathf.MoveTowards(moveSpeed, targetMovement, 8.0f * controller.deltaTime);

            horizontalMovement = Math3d.ProjectVectorOnPlane(lookDirection, input.Current.MoveInput) * 220.0f * controller.deltaTime;

        }
        else
        {
            moveSpeed = Mathf.MoveTowards(moveSpeed, 0, 8.0f * controller.deltaTime);
        }

        verticalMoveSpeed = Mathf.MoveTowards(verticalMoveSpeed, -30.0f, 35.0f * controller.deltaTime);

        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed) + controller.up * verticalMoveSpeed + horizontalMovement;

        Vector3 r = Vector3.Cross(lookDirection, controller.up);

        float ratio = verticalMoveSpeed < 0 ? Mathf.Abs(verticalMoveSpeed / 20.0f) : 0;
        float angle = Mathf.Lerp(0, 70.0f, ratio);

        artUpDirection = Quaternion.AngleAxis(-angle, r) * controller.up;
    }

    void Dive_ExitState() {
        artUpDirection = controller.up;

        controller.EnableClamping();
        controller.EnableSlopeLimit();
    }

    /// <summary>
    /// Italian Spiderman doing a Sideflip
    /// </summary>

    void SFlip_EnterState() {
        anim.Play("forward_flip");

        sound.PlaySlideSpin();

        currentJumpProfile = fallProfile;

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        verticalMoveSpeed = 9.0f;

        jumpPeak = transform.position;

        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed) + controller.up * verticalMoveSpeed;
    }

    void SFlip_SuperUpdate() {
        if (verticalMoveSpeed < 0 && AcquiringGround())
        {
            verticalMoveSpeed = 0;
            currentState = PlayerStates.Land;
            return;
        }

        SuperCollision col;

        if (HasWallCollided(out col) && WallCollisionAngle(col.normal, moveDirection) < 65.0f)
        {
            moveSpeed = runSpeed * 0.2f;
        }

        verticalMoveSpeed = Mathf.MoveTowards(verticalMoveSpeed, -currentJumpProfile.MaximumGravity, currentJumpProfile.Gravity * controller.deltaTime);

        if (Timer(timeEnteredState, anim["forward_flip"].length - 0.15f))
        {
            if (!anim.IsPlaying("falling_idle"))
                anim.CrossFade("falling_idle", 0.15f);
        }

        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed) + controller.up * verticalMoveSpeed;
    }

    void SFlip_ExitState() {
        controller.EnableClamping();
        controller.EnableSlopeLimit();

        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed) + controller.up * verticalMoveSpeed;
    }

    /// <summary>
    /// Italian Spiderman falling
    /// </summary>

    private Vector3 wallHitNormal;
    private float wallHitTime;

    void Fall_EnterState() {
        currentJumpProfile = fallProfile;

        jumpPeak = transform.position;

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        anim.CrossFade("falling_idle", 0.1f);
    }

    void Fall_SuperUpdate() {
        if (AcquiringGround())
        {
            verticalMoveSpeed = 0;

            currentState = PlayerStates.Land;

            return;
        }

        SuperCollision col;

        if (HasWallCollided(out col) && WallCollisionAngle(col.normal, moveDirection) < 65.0f)
        {
            moveSpeed = runSpeed * 0.2f;
        }

        if (input.Current.JumpDown && Time.time < wallHitTime + 0.2f)
        {
            Instantiate(WallKickSmokeEffect, transform.position + controller.up * (controller.height + controller.radius * 2) * 0.75f, Quaternion.LookRotation(wallHitNormal));

            lookDirection *= -1;
            moveSpeed = 0;
            currentJumpProfile = jumpWall;
            currentState = PlayerStates.Jump;
            return;
        }

        verticalMoveSpeed = Mathf.MoveTowards(verticalMoveSpeed, -25.0f, 35.0f * controller.deltaTime);

        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed) + controller.up * verticalMoveSpeed;
    }

    void Fall_ExitState() {
        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed) + controller.up * verticalMoveSpeed;

        controller.EnableClamping();
        controller.EnableSlopeLimit();
    }

    /// <summary>
    /// Italian Spiderman is knocked back/forward/air
    /// </summary>

    void Knockback_EnterState() {
        anim.Play("hit_to_body");
    }

    void Knockback_SuperUpdate() {
        if (!MaintainingGround())
        {
            currentState = PlayerStates.AirKnockback;
            return;
        }

        moveSpeed = Mathf.MoveTowards(moveSpeed, 0, 3.0f * controller.deltaTime);

        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed);

        if (moveSpeed == 0)
        {
            if (status.CurrentHealth == 0)
            {
                currentState = PlayerStates.DeathFront;
                return;
            }
            else
            {
                currentState = PlayerStates.KnockbackRecover;
                return;
            }
        }
    }

    void KnockbackForwards_EnterState() {
        if (anim.IsPlaying("hit_to_body"))
        {
            anim["hit_to_body"].time = 1.21f;
        }

        anim.Play("hit_to_body");
    }

    void KnockbackForwards_SuperUpdate() {
        if (!MaintainingGround())
        {
            currentState = PlayerStates.AirKnockbackForwards;
            return;
        }

        moveSpeed = Mathf.MoveTowards(moveSpeed, 0, 3.0f * controller.deltaTime);

        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed);

        if (moveSpeed == 0)
        {
            if (status.CurrentHealth == 0)
            {
                currentState = PlayerStates.DeathBack;
                return;
            }
            else
            {
                currentState = PlayerStates.KnockbackForwardsRecover;
                return;
            }
        }
    }

    void AirKnockback_EnterState() {
        anim.Play("hit_to_body");

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        verticalMoveSpeed = -1.0f;
    }

    void AirKnockback_SuperUpdate() {
        if (AcquiringGround())
        {
            verticalMoveSpeed = 0;

            controller.EnableClamping();
            controller.EnableSlopeLimit();

            Instantiate(GroundFallSmokeEffect, transform.position, Quaternion.identity);

            sound.PlayLandHurt();

            currentState = PlayerStates.Knockback;

            return;
        }

        verticalMoveSpeed = Mathf.MoveTowards(verticalMoveSpeed, -25.0f, 35.0f * controller.deltaTime);

        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed) + controller.up * verticalMoveSpeed;
    }

    void AirKnockbackForwards_EnterState() {
        anim.Play("hit_to_body");

        controller.DisableClamping();
        controller.DisableSlopeLimit();

        verticalMoveSpeed = -1.0f;
    }

    void AirKnockbackForwards_SuperUpdate() {
        if (AcquiringGround())
        {
            verticalMoveSpeed = 0;

            controller.EnableClamping();
            controller.EnableSlopeLimit();

            Instantiate(GroundFallSmokeEffect, transform.position, Quaternion.identity);

            sound.PlayLandHurt();

            currentState = PlayerStates.KnockbackForwards;

            return;
        }

        verticalMoveSpeed = Mathf.MoveTowards(verticalMoveSpeed, -25.0f, 35.0f * controller.deltaTime);

        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed) + controller.up * verticalMoveSpeed;
    }

    /// <summary>
    /// Italian Spiderman crouching -------------> delete
    /// </summary>

    private bool exitCrouch;
    private float crouchExitTime;

    void Crouch_EnterState() {
        exitCrouch = false;

        anim.CrossFade("crouching_idle", 0.1f);
    }

    void Crouch_SuperUpdate() {
        if (!MaintainingGround())
        {
            currentState = PlayerStates.Fall;
            return;
        }

        if (IsSliding())
        {
            currentState = PlayerStates.Slide;
            return;
        }

        if (!exitCrouch)
        {
            if (input.Current.JumpDown)
            {
                if (moveSpeed != 0 && !Timer(timeEnteredState, 0.4f))
                {
                    currentJumpProfile = jumpLong;
                    currentState = PlayerStates.Jump;
                }
                else if (moveSpeed != 0)
                {
                    currentJumpProfile = jumpStandard;
                    currentState = PlayerStates.Jump;
                }
                else
                {
                    currentJumpProfile = jumpBackFlip;
                    currentState = PlayerStates.Jump;
                }
                return;
            }

            if (Time.time > timeEnteredState + anim["crouching_idle"].length)
            {
                if (!anim.IsPlaying("crouching_idle")) //idle
                    anim.Play("crouching_idle");

                if (!input.Current.KickDown && moveDirection == Vector3.zero)
                {
                    exitCrouch = true;
                    crouchExitTime = Time.time;

                    anim["crouching_idle"].speed = -1;
                    anim["crouching_idle"].time = anim["crouching_idle"].length;
                    anim.Play("crouching_idle");

                    return;
                }

                if (GroundAngle() > 30.0f)
                {
                    moveDirection = Vector3.MoveTowards(moveDirection, SlopeDirection() * 13.0f, controller.deltaTime * 30.0f);
                }
                else
                {
                    moveSpeed = Mathf.MoveTowards(moveSpeed, 0, 15 * controller.deltaTime);

                    moveDirection = Math3d.SetVectorLength(moveDirection, Mathf.Abs(moveSpeed));
                }
            }
        }
        else
        {
            if (Time.time > crouchExitTime + anim["crouching_idle"].length)
            {
                exitCrouch = false;
                anim["crouching_idle"].speed = 1;
                anim.Play("weight_shift");
                currentState = PlayerStates.Idle;
                return;
            }
        }
    }

    /// <summary>
    /// Italian Spiderman hanging
    /// </summary>

    void Hang_EnterState() {
        moveDirection = Vector3.zero;

        sound.PlayHang();

        anim.Play("hanging_idle");
        AnimatedMesh.transform.Translate(Vector3.up * (-1.8f));
    }

    void Hang_SuperUpdate() {
        if (input.Current.JumpDown) {
            currentState = PlayerStates.Climb;
            return;
        }

        if (input.Current.KickDown) {
            moveSpeed = -2.0f;
            verticalMoveSpeed = -3.0f;
            currentState = PlayerStates.Fall;
            controller.currentlyClampedTo = null;

            AnimatedMesh.transform.Translate(Vector3.up * 1.8f);
            sound.PlayWallHit();

            return;
        }

        Vector3 ledgePosition;
        GameObject grabbedLedge;

        if (!CanGrabLedge(out ledgePosition, out grabbedLedge)) {
            moveSpeed = -2.0f;
            verticalMoveSpeed = -3.0f;
            currentState = PlayerStates.Fall;
            controller.currentlyClampedTo = null;
            AnimatedMesh.transform.Translate(Vector3.up * 1.8f);
            return;
        }

        if (SuperMath.Timer(timeEnteredState, 0.25f))
        {
            if (input.Current.MoveInput != Vector3.zero)
            {
                if (Vector3.Angle(lookDirection, input.Current.MoveInput) < 90.0f)
                {
                    currentState = PlayerStates.Climb;
                    return;
                }
                else
                {
                    moveSpeed = -2.0f;
                    verticalMoveSpeed = -3.0f;
                    currentState = PlayerStates.Fall;
                    controller.currentlyClampedTo = null;

                    AnimatedMesh.transform.Translate(Vector3.up * 1.8f);
                    sound.PlayWallHit();
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Italian Spiderman climbing
    /// </summary>

    void Climb_EnterState() {
        anim["climbing"].speed = 1.5f; //climb

        sound.PlayClimb();

        anim.CrossFade("climbing", 0.1f);
    }

    void Climb_SuperUpdate() {
        if (SuperMath.Timer(timeEnteredState, anim["climbing"].length / 1.5f))
        {
            transform.position = ClimbTarget();

            anim.Play("weight_shift");

            currentState = PlayerStates.Idle;
        }
    }

    void Climb_ExitState() {
        controller.EnableClamping();
        controller.EnableSlopeLimit();

        controller.currentlyClampedTo = null;
        AnimatedMesh.transform.Translate(Vector3.up * 1.8f);
    }

    /// <summary>
    /// Italian Spiderman recovering from sliding
    /// </summary>

    void SlideRecover_EnterState() {
        anim.Play("jump");
    }

    void SlideRecover_SuperUpdate() {
        if (!MaintainingGround()) {
            currentState = PlayerStates.Fall;
            return;
        }

        if (Timer(timeEnteredState, anim["jump"].length)) {
            currentState = PlayerStates.Idle;
        }
    }

    /// <summary>
    /// Italian Spiderman was knocked
    /// </summary>

    void KnockbackRecover_EnterState() {
        anim.Play("jump");
    }

    void KnockbackRecover_SuperUpdate() {
        if (!MaintainingGround()) {
            currentState = PlayerStates.Fall;
            return;
        }

        if (Timer(timeEnteredState, anim["jump"].length)) {
            currentState = PlayerStates.Idle;
        }
    }

    void KnockbackRecover_ExitState() {
        status.StartInvincible();
    }

    void KnockbackForwardsRecover_EnterState() {
        anim.Play("jump");
    }

    void KnockbackForwardsRecover_SuperUpdate() {
        if (!MaintainingGround()) {
            currentState = PlayerStates.Fall;
            return;
        }

        if (Timer(timeEnteredState, anim["jump"].length)) {
            currentState = PlayerStates.Idle;
        }
    }

    void KnockbackForwardsRecover_ExitState() {
        status.StartInvincible();
    }

    /// <summary>
    /// Italian Spiderman preparing pound ground ----------------------> delete
    /// </summary>

    //void GroundPoundPrepare_EnterState() {
    //    anim.Play("forward_flip");

    //    sound.StopVoices();
    //    sound.PlaySpin();

    //    verticalMoveSpeed = 2.0f;
    //    moveSpeed = 0.0f;

    //    jumpPeak = transform.position;
    //}

    //void GroundPoundPrepare_SuperUpdate()
    //{
    //    moveDirection = controller.up * verticalMoveSpeed;

    //    if (Timer(timeEnteredState, anim["forward_flip"].length))
    //    {
    //        currentState = PlayerStates.GroundPound;
    //    }
    //}

    //void GroundPound_EnterState()
    //{
    //    sound.PlayGroundPoundDown();

    //    verticalMoveSpeed = -20.0f;
    //}

    //void GroundPound_SuperUpdate()
    //{
    //    moveDirection = controller.up * verticalMoveSpeed;

    //    GameObject struckObject;

    //    if (FootStrike(out struckObject))
    //    {
    //        var machine = struckObject.GetComponent<EnemyMachine>();

    //        if (machine)
    //            machine.KillEnemy();
    //    }

    //    bool alreadyTriggered = false;

    //    if (AcquiringGround())
    //    {
    //        if (!alreadyTriggered)
    //        {
    //            var trigger = controller.currentGround.Transform.GetComponent<TriggerableObject>();

    //            if (trigger != null)
    //            {
    //                if (trigger.GroundPound())
    //                {
    //                    return;
    //                }
    //            }
    //        }

    //        currentState = PlayerStates.GroundPoundRecover;
    //        return;
    //    }
    //}

    //void GroundPoundRecover_EnterState()
    //{
    //    controller.EnableClamping();
    //    controller.EnableSlopeLimit();

    //    Instantiate(GroundPoundSmokeEffect, transform.position + controller.up * 0.15f, Quaternion.identity);

    //    sound.PlayGroundPound();

    //    if (ShouldHaveFallDamage())
    //    {
    //        if (FallDamage())
    //        {
    //            currentState = PlayerStates.Knockback;
    //            return;
    //        }
    //    }

    //    verticalMoveSpeed = 0.0f;
    //    moveDirection = controller.up * verticalMoveSpeed;

    //    anim.Play("jump");

    //    SmartCamera.Shake(0.25f, 5.0f, 0.25f);
    //}

    //void GroundPoundRecover_SuperUpdate()
    //{
    //    if (!MaintainingGround())
    //    {
    //        currentState = PlayerStates.Fall;
    //        return;
    //    }

    //    if (Timer(timeEnteredState, anim["jump"].length * 0.25f))
    //    {
    //        currentState = PlayerStates.Idle;
    //    }
    //}

    /// <summary>
    /// Italian Spiderman striking (punch)
    /// </summary>

    int strikeCount = 0;

    void Strike_EnterState() {
        strikeCount++;

        if (strikeCount == 1 && moveSpeed == 0) {
            moveSpeed = runSpeed / 3.0f;
        }

        anim.Rewind(ResolveStrike());
        anim.Play(ResolveStrike());

        if (strikeCount == 1)
            sound.PlaySinglePunch();
        else if (strikeCount == 2)
            sound.PlayDoublePunch();
        else
            sound.PlayKick();

        hasStruck = false;
    }

    void Strike_SuperUpdate() {
        if (!MaintainingGround()) {
            currentState = PlayerStates.Fall;
            return;
        }

        float decceleration = SuperMath.BoundedInterpolation(new float[] { 0 }, new float[] { 80, 10 }, moveSpeed);
        moveSpeed = Mathf.MoveTowards(moveSpeed, 0, decceleration * controller.deltaTime);

        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed);

        if (!hasStruck && Timer(timeEnteredState, anim[ResolveStrike()].length - 0.12f)) {
            GameObject struckObject;

            if (Strike(out struckObject, GetPunchOrigin(), GetPunchOffset())) {
                TriggerableObject trigger = struckObject.GetComponent<TriggerableObject>();
                EnemyMachine machine = struckObject.GetComponent<EnemyMachine>();

                if (trigger != null) {
                    trigger.Strike();

                    Instantiate(EnemyHitEffect, GetPunchPosition(), Quaternion.LookRotation(-lookDirection));
                }

                if (machine != null) {
                    if (goldPlayer) {
                        machine.MakeGold();
                    }

                    machine.GetStruck(lookDirection, 20.0f, 5.0f);

                    Instantiate(EnemyHitEffect, GetPunchPosition(), Quaternion.LookRotation(-lookDirection));
                }
                else {
                    Instantiate(EnemyHitEffect, GetPunchPosition(), Quaternion.LookRotation(-lookDirection));
                }

                sound.PlayImpact();
                hasStruck = true;
                moveSpeed = -13.0f;
            }
        }

        if (Timer(timeEnteredState, anim[ResolveStrike()].length)) {
            if (!anim.IsPlaying("hit_to_body")) {
                anim.CrossFade("hit_to_body", 0.5f);
            }

            if (input.Current.StrikeDown && strikeCount != 3) {
                currentState = PlayerStates.Strike;
                return;
            }
        }

        if (Timer(timeEnteredState, anim[ResolveStrike()].length + 0.2f)) {
            strikeCount = 0;
            currentState = PlayerStates.Idle;
            return;
        }
    }

    /// <summary>
    /// Italian Spiderman striking (kick)
    /// </summary>

    int kickCount = 0;

    void Kick_EnterState() {
        kickCount++;

        if (kickCount == 1 && moveSpeed == 0) {
            moveSpeed = runSpeed / 3.0f;
        }

        anim.Rewind(ResolveKick());
        anim.Play(ResolveKick());

        // cases for different kick sounds here
        sound.PlayKick();
        //if (kickCount == 1)
        //    sound.PlaySinglePunch();
        //else if (kickCount == 2)
        //    sound.PlayDoublePunch();
        //else
        //    sound.PlayKick();

        hasStruck = false;
    }

    void Kick_SuperUpdate() {
        if (!MaintainingGround()) {
            currentState = PlayerStates.Fall;
            return;
        }

        float decceleration = SuperMath.BoundedInterpolation(new float[] { 0 }, new float[] { 80, 10 }, moveSpeed);
        moveSpeed = Mathf.MoveTowards(moveSpeed, 0, decceleration * controller.deltaTime);

        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed);

        // did we hit anything within animation time?
        if (!hasStruck && Timer(timeEnteredState, anim[ResolveKick()].length - 0.12f)) {
            GameObject struckObject;

            if (Strike(out struckObject, GetKickOrigin(), GetKickOffset())) {
                TriggerableObject trigger = struckObject.GetComponent<TriggerableObject>();
                EnemyMachine machine = struckObject.GetComponent<EnemyMachine>();

                if (trigger != null) {
                    trigger.Strike();

                    Instantiate(EnemyHitEffect, GetPunchPosition(), Quaternion.LookRotation(-lookDirection));
                }

                if (machine != null) {
                    machine.GetStruck(lookDirection, 20.0f, 5.0f);

                    Instantiate(EnemyHitEffect, GetPunchPosition(), Quaternion.LookRotation(-lookDirection));
                }
                else {
                    Instantiate(EnemyHitEffect, GetPunchPosition(), Quaternion.LookRotation(-lookDirection));
                }

                sound.PlayImpact();
                hasStruck = true;
                // recoil/slow/stuck
                moveSpeed = 0.0f;
            }
        }

        if (Timer(timeEnteredState, anim[ResolveKick()].length)) {
            if (!anim.IsPlaying("hit_to_body")) {
                anim.CrossFade("hit_to_body", 0.5f);
            }

            if (input.Current.KickDown && kickCount != 3) {
                currentState = PlayerStates.Kick;
                return;
            }
        }

        if (Timer(timeEnteredState, anim[ResolveKick()].length + 0.2f)) {
            kickCount = 0;
            currentState = PlayerStates.Idle;
            return;
        }
    }



    bool staggerForward;

    void Stagger_EnterState()
    {
        controller.EnableClamping();
        controller.EnableSlopeLimit();

        if (staggerForward)
        {
            anim.CrossFade("hard_kick", 1.15f);
        }
        else
        {
            anim.CrossFade("hard_kick", 1.15f);
        }
    }

    void Stagger_SuperUpdate()
    {
        if (!MaintainingGround())
        {
            currentState = PlayerStates.Fall;
            return;
        }

        moveSpeed = Mathf.MoveTowards(moveSpeed, 0, 8.0f * controller.deltaTime);

        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed);

        if (moveSpeed == 0)
        {
            currentState = PlayerStates.Idle;
            return;
        }
    }

    void Stagger_ExitState()
    {
        status.StartInvincible();
    }

    Vector3 teleportTarget;

    void TeleportOut_EnterState()
    {
        moveSpeed = 0;
        verticalMoveSpeed = 0;
        moveDirection = Vector3.zero;

        anim.CrossFade("weight_shift", 0.3f);

        if (!goldPlayer)
        {
            transparencyShaderSwapper.SwapNew();
            transparencyFade.FadeOut(1);
        }

        sound.PlayTeleport();

        GameObject.FindObjectOfType<GameMaster>().FadeWhiteMatteOut(1.5f);
    }

    void TeleportOut_SuperUpdate()
    {
        if (SuperMath.Timer(timeEnteredState, 2))
        {
            transform.position = teleportTarget;

            var boulders = GameObject.FindObjectsOfType<RollingBallPath>();

            foreach (var boulder in boulders)
            {
                if (Vector3.Distance(boulder.transform.position, transform.position) < 10.0f)
                {
                    Destroy(boulder.gameObject);
                }
            }

            currentState = PlayerStates.TeleportIn;
            return;
        }
    }

    void TeleportIn_EnterState()
    {
        if (!goldPlayer)
            transparencyFade.FadeIn(1);

        sound.PlayTeleport();

        GameObject.FindObjectOfType<GameMaster>().FadeWhiteMatteIn(1.5f);
    }

    void TeleportIn_SuperUpdate()
    {
        if (SuperMath.Timer(timeEnteredState, 1))
        {
            currentState = PlayerStates.Idle;
            return;
        }
    }

    void TeleportIn_ExitState()
    {
        if (!goldPlayer)
            transparencyShaderSwapper.SwapOriginal();
    }

    void EnterLevel_EnterState()
    {
        anim["falling"].speed *= 1.5f;

        anim.Play("falling");

        sound.PlayFlipIntoLevel();

        // verticalMoveSpeed = -5.0f;

        moveDirection = verticalMoveSpeed * controller.up;

        controller.DisableClamping();
        controller.DisableSlopeLimit();
    }

    void EnterLevel_SuperUpdate()
    {
        jumpPeak = transform.position;

        verticalMoveSpeed = Mathf.MoveTowards(verticalMoveSpeed, -jumpStandard.MaximumGravity, jumpStandard.Gravity * controller.deltaTime);

        moveDirection = verticalMoveSpeed * controller.up;

        if (AcquiringGround())
        {
            currentState = PlayerStates.Land;
            return;
        }
    }

    void MegaSpring_EnterState()
    {
        controller.DisableClamping();
        controller.DisableSlopeLimit();

        anim.Play(jumpDouble.Animation);
    }

    void MegaSpring_SuperUpdate()
    {
        jumpPeak = transform.position;

        if (verticalMoveSpeed < 0 && AcquiringGround())
        {
            currentState = PlayerStates.Land;
            return;
        }

        if (!anim.IsPlaying(jumpDouble.FallAnimation))
        {
            anim.CrossFade(jumpDouble.FallAnimation, 0.2f);
        }

        verticalMoveSpeed = Mathf.MoveTowards(verticalMoveSpeed, -jumpStandard.MaximumGravity, jumpStandard.Gravity * controller.deltaTime);

        moveDirection = Math3d.SetVectorLength(lookDirection, moveSpeed) + controller.up * verticalMoveSpeed;
    }

    void DeathFront_EnterState()
    {
        anim.Play("stunned");

        sound.PlayDie();

        GameObject.FindObjectOfType<GameMaster>().GameOver();
    }

    void DeathBack_EnterState()
    {
        anim.Play("stunned");

        sound.PlayDie();

        GameObject.FindObjectOfType<GameMaster>().GameOver();
    }
}
