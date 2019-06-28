﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*
 * BaseAttack 객체의 변수와 Propertie 입니다.
 */
public abstract partial class BaseAttack : MonoBehaviourPun
{ 
    [SerializeField]
    protected float projectileSpeed = 10.0f;
    [SerializeField]
    protected int attackDamage = 0;
    [SerializeField]
    protected float destroyTime = 10;                 // 삭제까지의 대기시간

    protected Vector3 direction = default;

    #region Propertie
    public float ProjectileSpeed { get => projectileSpeed; }
    public int AttackDamage { get => attackDamage; }
    public float DestroyTime { get => destroyTime; }
    public Vector3 Direction { get => direction; }
    #endregion

    public new Rigidbody rigidbody { get; protected set; }
    public BasePlayer ownerPlayer { get; private set; }
}


public abstract partial class BaseAttack : MonoBehaviourPun
{

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public virtual void Cast(BasePlayer inOwnerPlayer, int inAttackDamage, Vector3 inDirection)
    {
        ownerPlayer = inOwnerPlayer;
        attackDamage = inAttackDamage;
        direction = inDirection;

        transform.rotation = Quaternion.LookRotation(inDirection);

        if (photonView.IsMine)
            StartCoroutine(Timer());
    }

    // 공격방식마다 추가적으로 해야할 작업이 있다면 이 함수를 오버라이딩 하세요
    public virtual void GiveAttack(BasePlayer player)
    {
        player.OnDamaged(attackDamage);

        // 총알 삭제구문. 
        
    }

    protected bool IsAttackable(Photon.Realtime.Player me, Photon.Realtime.Player enumy)
    {
        bool Attackable = false; ;

        Enums.TeamOption myTeamOption = (Enums.TeamOption)me.CustomProperties[Enums.PlayerProperties.TEAM.ToString()];
        Enums.TeamOption enumyTeamOption = (Enums.TeamOption)enumy.CustomProperties[Enums.PlayerProperties.TEAM.ToString()];

        switch (myTeamOption)
        {
            case Enums.TeamOption.RedTeam:
                if (enumyTeamOption == Enums.TeamOption.BlueTeam)
                    Attackable = true;
                break;
            case Enums.TeamOption.BlueTeam:
                if (enumyTeamOption == Enums.TeamOption.RedTeam)
                    Attackable = true;
                break;
            case Enums.TeamOption.Solo:
                Attackable = true;
                break;
        }
        return Attackable;
    }


    public void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine == false)
            return;

        if (other.CompareTag("Player"))
        {
            BasePlayer player = other.GetComponent<BasePlayer>();
            BaseCollisionProcess(player);
        }
    }

    public virtual void BaseCollisionProcess(BasePlayer player)
    {
        if (IsAttackable(PhotonNetwork.LocalPlayer, player.photonView.Owner))
        {
            player.OnDamaged(AttackDamage);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(destroyTime);

        if (photonView.IsMine) PhotonNetwork.Destroy(this.gameObject);

        yield break;
    }
}
