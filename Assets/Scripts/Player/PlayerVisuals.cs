﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
	public class PlayerVisuals : MonoBehaviour
	{
		[Header("Visual Settings")] 
		[SerializeField] private GameObject spriteContainer;
		[Header("Death Settings")] public GameObject DeathNugget;
		public int disectionCount;
		public float explosionForce;

		private PlayerMovement _pm;
		private Animator _anim;
		private static readonly int DeadAnimProp = Animator.StringToHash("Dead");
		private static readonly int DirAnimProp = Animator.StringToHash("Dir");
		private static readonly int WallClingAnimProp = Animator.StringToHash("WallCling");

		[Header("Aim Settings")] public GameObject aim;
		public float radius;
		private void Awake()
		{
			_pm = GetComponent<PlayerMovement>();
			_anim = GetComponent<Animator>();
		}

		public void DisableSprite()
		{
			spriteContainer.SetActive(false);
		}

		private void Update()
		{
			_anim.SetBool(DeadAnimProp,_pm.PlayerState == PlayerState.Dead);
			if (_pm.PlayerState == PlayerState.Flying)
			{
				_anim.SetFloat(DirAnimProp, _pm.Rigidbody.velocity.normalized.x);
			}
			_anim.SetBool(WallClingAnimProp,_pm.PlayerState == PlayerState.WallCling);
			_anim.SetBool("Idle", _pm.PlayerState == PlayerState.Inactive);
			_anim.SetBool("Trapped", _pm.PlayerState == PlayerState.Trapped);

			
			//update aim
			aim.transform.localPosition = _pm.Aim * radius;
			aim.transform.rotation = Quaternion.LookRotation(Vector3.back, _pm.Aim);
			aim.SetActive(ShowAim());
		}

		private bool ShowAim()
		{
			switch (_pm.PlayerState)
			{
				case PlayerState.Trapped:
				case PlayerState.Inactive:
					return true;
				case PlayerState.Dead:
				case PlayerState.Flying:
				case PlayerState.WallCling:
				default:
						return false;
			}
		}

		public void Explode()
		{
			DisableSprite();
			List<Rigidbody2D> nuggets = new List<Rigidbody2D>();
			Vector3 scale = transform.localScale / disectionCount;
			for (int i = 0; i < disectionCount; i++)
			{
				for (int j = 0; j < disectionCount; j++)
				{
					var n = Instantiate(DeathNugget);
					nuggets.Add(n.GetComponent<Rigidbody2D>());
					Vector3 offset = new Vector2(scale.x * i + scale.x / 2 - transform.localScale.x / 2,
						scale.y * j + scale.y / 2 - transform.localScale.y / 2);
					n.transform.localScale = scale;
					n.transform.position = transform.position + offset;
				}
			}

			foreach (var n in nuggets)
			{
				n.velocity = _pm.Rigidbody.velocity;
				n.AddForce(Random.insideUnitCircle * Random.insideUnitCircle * explosionForce, ForceMode2D.Impulse);
			}
		}
	}
}