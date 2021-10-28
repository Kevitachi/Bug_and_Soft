﻿using System;
using System.Collections;
using Controllers.Characters.Vergen.States;
using Controllers.Damage;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers.Characters.Vergen
{
    public class VergenController : BaseController
    {
        private bool _CombatEnabled;
        
        [SerializeField] private GameObject _SpearObject;
        [SerializeField] private Transform _SpearPosition;
        [SerializeField] private LayerMask _WhatIsSpikes;
        
        [SerializeField] private GameObject _vergenGhost;
        [SerializeField] private AudioSource _vergenTrap1SFX;
        [SerializeField] private AudioSource _vergenTrap2SFX;
        [SerializeField] private AudioSource _vergenBattleShoutSFX;
        [SerializeField] private AudioSource _vergenTpSFX;
        
        public VergenGhostTrigger[] VergenGhostTriggers;

        public VergenIdleState vergenIdleState { get; private set; }
        public VergenPhaseOneState vergenPhaseOneState { get; private set; }
        public VergenPhaseTwoState vergenPhaseTwoState { get; private set; }
        public VergenNormalAttackState vergenNormalAttackState { get; private set; }
        public VergenLongRangeAttack vergenLongAttackState { get; private set; }
        
        public VergenTorbeshinoState vergenTorbeshinoState { get; private set; }
        
        public VergenFreeKillState vergenFreeKillState { get; private set; }
        
        public VergenTeleportState vergenTeleportState { get; private set; }
        
        public bool HasSpearAttackFinished { get; private set; }

        private bool spearShot;

        public int CurrentPhase = 1;
        public bool AlwaysEnterRageMode;
        
        protected override void Start()
        {
            base.Start();
            
            vergenIdleState = new VergenIdleState(this, StateMachine, "Idle", this);
            vergenPhaseOneState = new VergenPhaseOneState(this, StateMachine, "Run", this);
            vergenPhaseTwoState = new VergenPhaseTwoState(this, StateMachine, "Run", this);
            vergenNormalAttackState = new VergenNormalAttackState(this, StateMachine, "NormalAttack", this);
            vergenLongAttackState = new VergenLongRangeAttack(this, StateMachine, "LongRangeAttack", this);
            vergenTorbeshinoState = new VergenTorbeshinoState(this, StateMachine, "Torbeshino", this);
            vergenFreeKillState = new VergenFreeKillState(this, StateMachine, "", this);
            vergenTeleportState = new VergenTeleportState(this, StateMachine, "Disappear", this);
            
            vergenLongAttackState.OnVergenShootingSpears.AddListener(ShootSpears);
            GameManager.Instance.OnVergenTrappedPlayer.AddListener(VergenTrapStart);

            foreach (var ghostTrigger in VergenGhostTriggers)
            {
                ghostTrigger.OnVergenHeadDodged.AddListener(RestorePhaseTwo);
            }
            
            StateMachine.Initialize(vergenIdleState);
        }

        protected override void Update()
        {
            if (_CombatEnabled)
            {
                base.Update();
            }
        }

        protected override void FixedUpdate()
        {
            if (_CombatEnabled)
            {
                base.FixedUpdate();
            }
        }

        public void StartCombat()
        {
            _CombatEnabled = true;
            StateMachine.ChangeState(vergenPhaseOneState);
            
            GameManager.Instance.ShowBossHealth("VERGEN - FASE 1", this);
        }
        
        public void OnAttackFinish()
        {
            vergenNormalAttackState.OnAttackFinish();
        }

        public void PhaseOneAttackWait()
        {
            StartCoroutine(AttackWaitForExit());
        }
        
        private IEnumerator AttackWaitForExit()
        {
            yield return new WaitForSeconds(Random.Range(0f, 1f));
            StateMachine.ChangeState(vergenPhaseOneState);
        }
        
        public void SpawnSpear()
        {
            Quaternion rot;

            if (FacingDirection == 1)
                rot = Quaternion.Euler(0, 0, 0);
            else
                rot = Quaternion.Euler(0, 180, 0);
            
            GameObject proj = Instantiate(_SpearObject, _SpearPosition.position, rot);
            VergenSpear vSpear = proj.GetComponent<VergenSpear>();
            
            vSpear.FireProjectile(30f, 30f, false);

            vergenLongAttackState.OnSpearShot();
        }

        public void ShootSpears()
        {
            HasSpearAttackFinished = false;
            GetAnimator().SetBool("LongRangeAttack", true);
        }
        
        public void RestorePhaseTwo()
        {
            StateMachine.ChangeState(vergenPhaseTwoState);
        }

        private void VergenTrapStart()
        {
            StopAllCoroutines();
            StartCoroutine(VergenMasterTrap());
        }
        
        private IEnumerator VergenMasterTrap()
        {
            GameManager.Instance.GetDynamicCamera().UpdateSize(7.85f, 2f);
            yield return new WaitForSeconds(3f);
            
            StateMachine.ChangeState(vergenFreeKillState);
            rBody.velocity = Vector2.zero;
            transform.position = GameManager.Instance.PlayerController.VergenTrapTarget.position;
            LookAtPlayer();

            _vergenTrap1SFX.Play();
            yield return new WaitForSeconds(1f);
            
            GetAnimator().SetBool("NormalAttack", true);
            _vergenTrap2SFX.Play();
            _vergenBattleShoutSFX.Play();
            yield return new WaitForSeconds(0.15f);
            
            yield return 0;
        }

        private void Anim_OnSpearShotFinished()
        {
            HasSpearAttackFinished = true;
            GetAnimator().SetBool("LongRangeAttack", false);
            spearShot = true;
        }

        public void Anim_OnDisappearFinished()
        {
            vergenTeleportState.OnDisappearFinished();
        }

        public override void Damage(DamageInfo damageInfo)
        {
            
            if (Time.time > damagedTimeCD)
            {
                //_vergenGhostTriggers[Random.Range(0, 2)].Run();
                
                // Obtenemos la posicion del ataque
                int direction = damageInfo.GetAttackDirection(transform.position.x);

                playerFacingDirection = playerController.IsFacingRight();

                Instantiate(hitParticles, transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360f)));

                if (firstAttack)
                    hitAttackSFX1.Play();
                else
                    hitAttackSFX2.Play();

                firstAttack = !firstAttack;

                if (!isInvencible)
                    currentHealth -= damageInfo.DamageAmount;

                if (damageInfo.MoveOnAttack)
                    MoveOnDamaged(direction, damageInfo.MoveOnAttackForce);

                StartCoroutine(DamageEffect());

                if (currentHealth <= 0f)
                {
                    GivePlayerExp();
                    OnLifeTimeEnded.Invoke();
                    Die();
                }

                damagedTimeCD = Time.time + .15f;
            }
        }
    }
}