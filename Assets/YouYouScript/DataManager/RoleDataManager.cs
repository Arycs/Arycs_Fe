using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Models;
using UnityEngine;
using YouYou;

public class RoleDataManager : IDisposable
{
    public static ulong nextRoleGUID { get; private set; }

    private Dictionary<ClassType, MoveConsumption> m_MoveConsumptions;
    private Dictionary<int, Class> m_Classes;
    private Dictionary<int, Character> m_Characters;

    /// <summary>
    /// 独有角色
    /// </summary>
    private Dictionary<int, UniqueRole> m_UniqueRoles;

    /// <summary>
    /// 部下杂兵模板
    /// </summary>
    private Dictionary<int, RoleData> m_FollowingTemplates;

    /// <summary>
    /// 部下杂兵角色
    /// </summary>
    private Dictionary<ulong, FollowingRole> m_FollowingRoles;

    public void Init()
    {
        m_MoveConsumptions = new Dictionary<ClassType, MoveConsumption>();
        m_Classes = new Dictionary<int, Class>();
        m_Characters = new Dictionary<int, Character>();

        nextRoleGUID = 1UL;
        m_UniqueRoles = new Dictionary<int, UniqueRole>();
        m_FollowingTemplates = new Dictionary<int, RoleData>();
        m_FollowingRoles = new Dictionary<ulong, FollowingRole>();
    }

    /// <summary>
    /// 获取或创建移动消耗
    /// </summary>
    /// <param name="classType"></param>
    /// <returns></returns>
    public MoveConsumption GetOrCreateMoveConsumption(ClassType classType)
    {
        MoveConsumption consumption;
        if (!m_MoveConsumptions.TryGetValue(classType,out consumption))
        {
            Sys_MoveConsumptionEntity entity = GameEntry.DataTable.Sys_MoveConsumptionDBModel.Get((int)classType);
            if (entity == null)
            {
                Debug.LogErrorFormat("RoleDataManager -> MoveConsumption key '{0}' is not fount",classType.ToString());
                return null;
            }
            else
            {
                consumption = new MoveConsumption(entity);
                m_MoveConsumptions.Add(classType,consumption);
            }
        }
        return consumption;
    }

    /// <summary>
    /// 获取或创建职业
    /// </summary>
    /// <param name="classId"></param>
    /// <returns></returns>
    public Class GetOrCreateClass(int classId)
    {
        Class cls;
        if (!m_Classes.TryGetValue(classId,out cls))
        {
            Sys_ClassEntity entity = GameEntry.DataTable.Sys_ClassDBModel.Get(classId);
            if (entity == null)
            {
                Debug.LogErrorFormat("RoleDataManager -> Class key '{0}' is not fount",classId.ToString());
                return null;
            }
            else
            {
                cls = new Class(entity);
                m_Classes.Add(classId, cls);
            }
        }

        return cls;
    }

    /// <summary>
    /// 获取或创建独有人物
    /// </summary>
    /// <param name="characterId"></param>
    /// <returns></returns>
    public Character GetOrCreateCharacter(int characterId)
    {
        Character character;
        if (!m_Characters.TryGetValue(characterId,out character))
        {
            Sys_CharacterEntity entity = GameEntry.DataTable.Sys_CharacterDBModel.Get(characterId);
            if (entity == null)
            {
                Debug.LogErrorFormat("RoleDataManager -> Character key ‘{0}’ is not fount",characterId.ToString());
                return null;
            }
            else
            {
                character = new Character(entity);
                m_Characters.Add(characterId,character);
            }
        }

        return character;
    }

    /// <summary>
    /// 获取或创建杂兵模板
    /// </summary>
    /// <param name="classId"></param>
    /// <returns></returns>
    public RoleData GetOrCreateFollowingTemplate(int classId)
    {
        Class cls = GetOrCreateClass(classId);
        if (cls == null || cls.info == null)
        {
            return null;
        }

        Sys_ClassEntity info = cls.info;

        RoleData data;
        if (!m_FollowingTemplates.TryGetValue(info.Id,out data))
        {
            data = new RoleData {classId = info.Id};
            //TODO 计算公式， 计算NPC出生数据
            
            m_FollowingTemplates.Add(data.classId,data);
        }

        return data;
    }

    /// <summary>
    /// 获取或创建角色
    /// 独有角色：characterId
    /// 部下杂兵角色：classId
    /// </summary>
    /// <param name="id"></param>
    /// <param name="roleType"></param>
    /// <returns></returns>
    public Role GetOrCreateRole(int id, RoleType roleType)
    {
        if (roleType == RoleType.Unique)
        {
            UniqueRole role;
            if (!m_UniqueRoles.TryGetValue(id,out role))
            {
                role = CreateUniqueRole(id);
                if (role != null)
                {
                    m_UniqueRoles.Add(role.characterId,role);
                }
            }

            return role;
        }
        else
        {
            FollowingRole role = CreateFollowingRole(id);
            if (role!= null)
            {
                m_FollowingRoles.Add(role.guid,role);
            }

            return role;
        }
    }

    /// <summary>
    /// 创建独有角色
    /// </summary>
    /// <param name="characterId"></param>
    /// <returns></returns>
    private UniqueRole CreateUniqueRole(int characterId)
    {
        UniqueRole role = new UniqueRole();
        RoleData data = CreateUniqueRoleData(characterId);
        if (!role.Load(data))
        {
            return null;
        }

        return role;
    }
    
    /// <summary>
    /// 创建独有角色数据
    /// </summary>
    /// <param name="characterId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private RoleData CreateUniqueRoleData(int characterId)
    {
        Character character = GetOrCreateCharacter(characterId);
        if (character == null)
        {
            return null;
        }

        Class cls = GetOrCreateClass(character.info.CharacterClassId);
        if (cls == null)
        {
            return null;
        }

        RoleData self = new RoleData
        {
            characterId = characterId,
            classId = character.info.CharacterClassId,
            level = Mathf.Clamp(character.info.CharacterLevel, 0, SettingVars.maxLevel),
            exp = 0,
            fightProperties = FightProperties.Clamp(character.info.CharacterFightProperties + cls.info.FightProperties,
                cls.info.MaxFightProperties),
            hp = character.info.CharacterHp,
            mp = character.info.CharacterMp,
            luk = Mathf.Clamp(character.info.CharacterLuk, 0, SettingVars.maxLuk),
            movePoint = cls.info.MovePoint
        };

        return self;
    }

    /// <summary>
    /// 创建部下或杂兵
    /// </summary>
    /// <param name="classId"></param>
    /// <returns></returns>
    private FollowingRole CreateFollowingRole(int classId)
    {
        FollowingRole role = new FollowingRole(nextRoleGUID++);
        RoleData template = GetOrCreateFollowingTemplate(classId);
        if (!role.Load(template))
        {
            return null;
        }

        return role;
    }
    
    public void Dispose()
    {
        m_MoveConsumptions = null;
        m_Characters = null;
        m_Characters = null;

        m_UniqueRoles = null;
        m_FollowingTemplates = null;
        m_FollowingRoles = null;
    }
}
