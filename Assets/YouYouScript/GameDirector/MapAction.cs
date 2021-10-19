using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arycs_Fe.CombatManagement;
using Arycs_Fe.Maps;
using Arycs_Fe.Models;
using Arycs_Fe.ScriptManagement;
using UnityEngine;
using UnityEngine.UIElements;
using YouYou;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 地图动作类，用来执行地图脚本
    /// </summary>
    public class MapAction : GameAction
    {
        /// <summary>
        /// 当前执行脚本的地图
        /// </summary>
        public MapGraph Map { get; set; }
        
        /// <summary>
        /// 执行完的下一个场景
        /// </summary>
        private string NextScene { get; set; } = string.Empty;
        
        /// <summary>
        /// 当前脚本的状态
        /// </summary>
        public ActionStatus Status { get; set; } = ActionStatus.Error;
        
        /// <summary>
        /// 地图 剧情脚本
        /// </summary>
        public MapScenarioAction ScenarioAction = null;

        /// <summary>
        /// 地图状态,用来判断是显示攻击范围等---
        /// </summary>
        public MapStatus MapStatus { get; set; } = MapStatus.Normal;

        /// <summary>
        /// 当前回合为哪方阵营
        /// </summary>
        public AttitudeTowards Turn { get; set; } = AttitudeTowards.Player;
        
        /// <summary>
        /// 回合数,用来判断某些胜利条件或者事件
        /// </summary>
        public int TurnToken { get; set; } = 0;

        /// <summary>
        /// 当前选中的格子数据
        /// </summary>
        public CellData SelectedCell { get; set; } = null;
        
        /// <summary>
        /// 当前选中的实体(角色)
        /// </summary>
        public MapClass SelectedUnit { get; set; } = null;
        
        /// <summary>
        /// 当前的目标实体(角色)
        /// </summary>
        public MapClass TargetUnit { get; set; } = null;
        
        /// <summary>
        /// 移动结束格子信息
        /// </summary>
        private CellData MovingEndCell { get; set; } = null;

        /// <summary>
        /// 阵营以及 对应角色字典
        /// </summary>
        private readonly Dictionary<AttitudeTowards, List<MapClass>> m_UnitDict =
            new Dictionary<AttitudeTowards, List<MapClass>>();

        /// <summary>
        /// 障碍物集合
        /// </summary>
        protected readonly HashSet<MapObstacle> m_Obstacles = new HashSet<MapObstacle>();

        private MapEventCollection m_MapEvents = new MapEventCollection();

        public MapAction() : base()
        {
        }

        public MapAction(IGameAction gameAction) : base(gameAction)
        {
        }

        /// <summary>
        /// 读取地图
        /// </summary>
        /// <param name="scriptName"></param>
        /// <returns></returns>
        public bool Load(string scriptName)
        {
            //寻找地图
            Map = GameObject.FindObjectOfType<MapGraph>();
            if (Map == null)
            {
                error = "MapAction -> MapGraph was not found";
                return false;
            }

            Map.InitMap();
            
            TextAsset asset = new TextAsset();
            MapEventInfo info = null;
            GameEntry.Resource.ResourceLoaderManager.LoadMainAsset(AssetCategory.MapEventInfo,
                $"Assets/Download/MapConfig/{scriptName}.asset",(
                Resources =>
                {
                    info = Resources.Target as MapEventInfo;
                }));
            if (info == null)
            {
                error = $"MapAction -> 加载{scriptName}地图事件错误";
                return false;
            }

            //地图结束后的场景名称 可以为null
            // 为null时， 回到上一个场景
            NextScene = info.nextScene;

            //设置鼠标光标
            if (!string.IsNullOrEmpty(info.mouseCursor.prefab))
            {
                GameEntry.Pool.GameObjectPool.Spawn(PrefabId.MouseCursor,(transform =>
                {
                    Map.mouseCursor = transform.GetComponent<MapMouseCursor>();
                }));
            }

            Map.mouseCursor.UpdatePosition(new Vector3Int(info.mouseCursor.x, info.mouseCursor.y, 0));

            //设置移动范围和攻击范围的图标
            if (!string.IsNullOrWhiteSpace(info.cursor))
            {
                GameEntry.Pool.GameObjectPool.Spawn(PrefabId.MoveOrAttackCursor,(transform =>
                {
                    Map.cursorPrefab = transform.GetComponent<MapCursor>();
                }));
            }

            //读取剧情剧本与地图事件
            if (!LoadScenario(info.scenarioName) || !LoadMapEvent(info))
            {
                return false;
            }

            //设置状态
            Status = ActionStatus.WaitInput;
            return true;
        }

        /// <summary>
        /// 读取地图剧本
        /// </summary>
        /// <param name="scriptName"></param>
        /// <returns></returns>
        private bool LoadScenario(string scriptName)
        {
            //剧本可以为null ，地图中没有剧情，纯战斗
            if (string.IsNullOrEmpty(scriptName))
            {
                return true;
            }

            Iscenario scenario = new TxtScript();
            GameEntry.GameDirector.LoadScenarioAsset(scriptName,(resourceEntity =>
            {
                TextAsset textAsset = resourceEntity.Target as TextAsset;
                if (!(textAsset is null)) scenario.Load(scriptName, textAsset.text);
            }));

            MapScenarioAction action = new MapScenarioAction(this);
            List<Type> executorTypes = GameAction.GetDefaultExecutorTypesForScenarioAction();
            action.LoadExecutors(executorTypes.ToArray());
            if (!action.LoadScenario(scenario))
            {
                error = action.error;
                action.Dispose();
                return false;
            }

            ScenarioAction = action;
            return true;
        }

        /// <summary>
        /// 执行地图剧情
        /// </summary>
        public void ScenarioDone()
        {
            if (Status == ActionStatus.WaitScenarioDone)
            {
                Status = ActionStatus.WaitInput;
                GameEntry.GameDirector.StopGameAction();
            }
        }

        /// <summary>
        /// 开始剧情地图
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool ScenarioCommand(string flag)
        {
            if (ScenarioAction == null)
            {
                error = "MapAction -> No Scenario";
                return false;
            }

            ActionStatus s = ScenarioAction.GotoCommand(flag, out m_Error);
            if (s == ActionStatus.Error)
            {
                Status = ActionStatus.Error;
                return false;
            }

            Status = ActionStatus.WaitScenarioDone;
            GameEntry.GameDirector.RunGameAction();
            return true;
        }


        /// <summary>
        /// 创建Obstacle
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="cmdError"></param>
        /// <returns></returns>
        public ActionStatus ObjectCommandCreateObstacle(string prefab, Vector3Int position, out string cmdError)
        {
            CellData cellData = Map.GetCellData(position);
            if (cellData == null)
            {
                cmdError =
                    $"ObjectExecutor Create Obstacle -> position '{position.ToString()}' is out of range, prefab : {prefab} ";
                return ActionStatus.Error;
            }

            if (cellData.hasMapObject)
            {
                cmdError =
                    $"ObjectExecutor Create Obstacle -> the object in position '{position.ToString()}' is already, prefab :{prefab}";
                return ActionStatus.Error;
            }

            MapObject mapObject = Map.CreateMapObject(prefab, position);
            if (mapObject == null || mapObject.mapObjectType != MapObjectType.Obstacle)
            {
                cmdError = $"ObjectExecutor Create Obstacle -> create map object error. prefab :{prefab} ";
                if (mapObject != null)
                {
                    //TODO 回池
                    //ObjectPool.DespawnUnsafe(mapObject.gameObject,true);
                }

                return ActionStatus.Error;
            }

            m_Obstacles.Add(mapObject as MapObstacle);
            cellData.mapObject = mapObject;
            cmdError = null;
            return ActionStatus.Continue;
        }

        /// <summary>
        /// 创建Class
        /// </summary>
        /// <param name="attitudeTowards"></param>
        /// <param name="roleType"></param>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="level"></param>
        /// <param name="items"></param>
        /// <param name="cmdError"></param>
        /// <returns></returns>
        public ActionStatus ObjectCommandCreateClass(AttitudeTowards attitudeTowards, RoleType roleType, int id,
            Vector3Int position, int level, int[] items, out string cmdError)
        {
            if (roleType == RoleType.Following && attitudeTowards == AttitudeTowards.Player)
            {
                cmdError = $"ObjectExecutor Create Class -> role type of player can only be 'Unique'";
                return ActionStatus.Error;
            }

            CellData cellData = Map.GetCellData(position);
            if (cellData == null)
            {
                cmdError =
                    $"ObjectExecutor Create Class -> position '{position.ToString()}',id :{id.ToString()}";
                return ActionStatus.Error;
            }

            if (cellData.hasMapObject)
            {
                cmdError =
                    $"ObjectExecutor Create Class -> the object in position '{position.ToString()}' is already exist. id :{id.ToString()}";
                return ActionStatus.Error;
            }

            string prefab;
            if (roleType == RoleType.Unique)
            {
                Character character = GameEntry.Data.RoleDataManager.GetOrCreateCharacter(id);
                prefab = GameEntry.Data.RoleDataManager.GetOrCreateClass(character.info.CharacterClassId).info.Prefab;
            }
            else
            {
                prefab = GameEntry.Data.RoleDataManager.GetOrCreateClass(id).info.Prefab;
            }


            MapObject mapObject = Map.CreateMapObject(prefab, position);
            if (mapObject == null)
            {
                cmdError = $"{"ObjectExecutor"} Create Class -> create map object error";
                return ActionStatus.Error;
            }

            if (mapObject.mapObjectType != MapObjectType.Class)
            {
                cmdError =
                    $"ObjectExecutor Create Class -> Create map object error, type error, id:{id.ToString()}";
                //TODO 回池
                //ObjectPool.DespawnUnsafe(mapObject.gameObject, true);
                return ActionStatus.Error;
            }

            MapClass mapCls = mapObject as MapClass;
            if (!mapCls.Load(id, roleType))
            {
                cmdError =
                    $"ObjectExecutor Run -> load role error. id :{id.ToString()}, role type :{roleType.ToString()}";
                return ActionStatus.Error;
            }

            mapCls.role.attitudeTowards = attitudeTowards;
            mapCls.swapper.SwapColors(GetSwapperColorName(attitudeTowards));
            if (roleType == RoleType.Following)
            {
                mapCls.role.level = Mathf.Max(1, level);
                if (items != null && items.Length > 0)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        mapCls.role.AddItem(GameEntry.Data.ItemDataManager.CreateItem(items[i]));
                    }
                }
            }

            List<MapClass> classes;
            if (!m_UnitDict.TryGetValue(attitudeTowards, out classes))
            {
                classes = new List<MapClass>();
                m_UnitDict.Add(attitudeTowards, classes);
            }

            classes.Add(mapCls);
            cellData.mapObject = mapCls;
            cmdError = null;
            return ActionStatus.Continue;
        }

        /// <summary>
        /// 为独有角色创建MapClass
        /// </summary>
        /// <param name="attitudeTowards"></param>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="cmdError"></param>
        /// <returns></returns>
        public ActionStatus ObjectCommandCreateClassUnique(AttitudeTowards attitudeTowards, int id, Vector3Int position,
            out string cmdError)
        {
            return ObjectCommandCreateClass(attitudeTowards, RoleType.Unique, id, position, 0, null, out cmdError);
        }

        /// <summary>
        /// 为部下创建MapClass
        /// </summary>
        /// <param name="attitudeTowards"></param>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="level"></param>
        /// <param name="items"></param>
        /// <param name="cmdError"></param>
        /// <returns></returns>
        public ActionStatus ObjectCommandCreateClassFollowing(
            AttitudeTowards attitudeTowards,
            int id,
            Vector3Int position,
            int level,
            int[] items,
            out string cmdError)
        {
            return ObjectCommandCreateClass(
                attitudeTowards,
                RoleType.Following,
                id,
                position,
                level,
                items,
                out cmdError);
        }

        /// <summary>
        /// 点击左键
        /// </summary>
        /// <param name="mousePosition"></param>
        public override void OnMouseLButtonDown(Vector3 mousePosition)
        {
            if (Status == ActionStatus.WaitScenarioDone)
            {
                ScenarioAction.OnMouseLButtonDown(mousePosition);
                return;
            }

            if (!CanInput(false))
            {
                return;
            }

            //获取点击的CellData
            Vector3Int cellPosition = Map.mouseCursor.cellPosition;
            CellData cell = Map.GetCellData(cellPosition);
            if (cell == null)
            {
                return;
            }

            switch (MapStatus)
            {
                case MapStatus.Normal:
                    if (SelectedUnit == null)
                    {
                        // 如果Cell中有角色
                        if (cell.hasMapObject && cell.mapObject.mapObjectType == MapObjectType.Class)
                        {
                            MapClass mapClass = cell.mapObject as MapClass;
                            SelectedCell = cell;
                            SelectedUnit = mapClass;
                        }
                    }

                    ShowMapMenu(false);
                    break;
                case MapStatus.MoveCursor:
                    if (SelectedUnit.role.attitudeTowards != AttitudeTowards.Player)
                    {
                        Map.HideRangeCursors();
                        SelectedUnit = null;
                        SelectedCell = null;
                        MapStatus = MapStatus.Normal;
                        break;
                    }

                    //如果选中的不在移动范围内
                    if (!cell.hasMoveCursor)
                    {
                        break;
                    }

                    //如果选中的在移动范围内，那么开始移动，并播放移动动画
                    MoveMapClass(SelectedUnit, cell);
                    break;
                case MapStatus.AttackCursor:
                    //如果选中的不在攻击范围内
                    if (!cell.hasAttackCursor)
                    {
                        break;
                    }

                    //如果选中的不存在地图对象
                    if (!cell.hasMapObject)
                    {
                        break;
                    }

                    //如果选中的实体不是职业
                    if (cell.mapObject.mapObjectType != MapObjectType.Class)
                    {
                        break;
                    }

                    //如果选中的职业不是敌人
                    MapClass target = cell.mapObject as MapClass;
                    if (target.role.attitudeTowards != AttitudeTowards.Enemy)
                    {
                        break;
                    }

                    TargetUnit = target;
                    //攻击目标
                    AttackMapClass(SelectedUnit, target);
                    break;
            }
        }

        /// <summary>
        /// 显示地图菜单
        /// </summary>
        /// <param name="sub"></param>
        private void ShowMapMenu(bool sub)
        {
            //TODO 打开MapMenu 界面
            HashSet<MenuTextID> showButtons;

            // 如果点击的不是角色，那么显示主菜单
            if (SelectedUnit == null)
            {
                showButtons = GetDefaultMainMenuTextIds();
                //如果角色移动过，就不能存档
                foreach (MapClass mapClass in m_UnitDict[AttitudeTowards.Player])
                {
                    if (mapClass.role.holding)
                    {
                        showButtons.Remove(MenuTextID.Save);
                        break;
                    }
                }
            }
            else
            {
                showButtons = GetDefaultUnitMenuTextIds();
                //如果是玩家
                if (SelectedUnit.role.attitudeTowards == AttitudeTowards.Player)
                {
                    //如果是移动后的菜单
                    if (sub)
                    {
                        showButtons.Remove(MenuTextID.Move);
                        showButtons.Remove(MenuTextID.Status);
                        //如果物品栏里没有武器， 或所有武器不可用
                        bool canAtk = false;
                        for (int i = 0; i < SelectedUnit.role.items.Length; i++)
                        {
                            Item item = SelectedUnit.role.items[i];
                            if (item == null || item.ItemType != ItemType.Weapon)
                            {
                                continue;
                            }

                            //有武器，但是职业不能装备此武器
                            if ((item as Weapon).level >
                                SelectedUnit.role.cls.info.AvailableWeapons[(item as Weapon).weaponType])
                            {
                                continue;
                            }

                            canAtk = true;
                            break;
                        }

                        if (!canAtk)
                        {
                            showButtons.Remove(MenuTextID.Attack);
                        }

                        if (true /*TODO 检测 是否有对话*/)
                        {
                            showButtons.Remove(MenuTextID.Talk);
                        }
                    }
                    else //移动后菜单
                    {
                        //移动前不能待机
                        showButtons.Remove(MenuTextID.Holding);

                        //如果目标已经待机，不可移动
                        if (SelectedUnit.role.holding)
                        {
                            showButtons.Remove(MenuTextID.Move);
                        }

                        // 移动之后不能攻击和谈话
                        showButtons.Remove(MenuTextID.Attack);
                        showButtons.Remove(MenuTextID.Talk);
                    }
                }
                else //如果不是玩家，则只能显示移动范围和查看状态
                {
                    showButtons.Remove(MenuTextID.Holding);
                    showButtons.Remove(MenuTextID.Talk);
                    showButtons.Remove(MenuTextID.Attack);
                }
            }
        }

        /// <summary>
        /// 菜单按钮点击事件
        /// </summary>
        /// <param name="menuTextID"></param>
        private void MapMenu_OnButtonClick(MenuTextID menuTextID)
        {
            switch (menuTextID)
            {
                case MenuTextID.Unit:
                    // TODO
                    break;
                case MenuTextID.Close:
                    ResetSelected();
                    break;
                case MenuTextID.Move:
                    if (Map.SearchAndShowMoveRange(SelectedUnit, true))
                    {
                        MapStatus = MapStatus.MoveCursor;
                        DisplayMouseCursor(true);
                    }
                    else
                    {
                        MapStatus = MapStatus.Normal;
                        error = "MapAction -> Search Move Range error";
                        Status = ActionStatus.Error;
                    }

                    break;
                case MenuTextID.Attack:
                    //TODO 这里应该显示选择武器面板，后续修改
                    Weapon roleWeapon = SelectedUnit.role.equipedWeapon;
                    List<CellData> atkCells =
                        Map.SearchAttackRange(MovingEndCell, roleWeapon.minRange, roleWeapon.maxRange, true);
                    Map.ShowRangeCursors(atkCells, MapCursor.CursorType.Attack);
                    MapStatus = MapStatus.AttackCursor;
                    DisplayMouseCursor(true);
                    break;
                case MenuTextID.Holding:
                    HoldingMapClass(SelectedUnit);
                    break;
                case MenuTextID.TurnEnd:
                    NextTurn();
                    break;
                // 省略其它case
            }
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="mapClass"></param>
        /// <param name="moveTo"></param>
        protected void MoveMapClass(MapClass mapClass, CellData moveTo)
        {
            //隐藏光标
            Map.HideRangeCursors();
            DisplayMouseCursor(false);
            // 开始移动动画
            Stack<CellData> path = Map.searchPath.BuildPath(moveTo);
            mapClass.onMovingEnd += MapClass_OnMovingEnd;
            MapStatus = MapStatus.Animation;
            mapClass.animatorController.PlayMove();
            mapClass.StartMove(path);
        }

        private void MapClass_OnMovingEnd(CellData endCell)
        {
            SelectedUnit.onMovingEnd -= MapClass_OnMovingEnd;

            //设置坐标
            SelectedCell.mapObject = null;
            MovingEndCell = endCell;
            MovingEndCell.mapObject = SelectedUnit;
            SelectedUnit.UpdatePosition(MovingEndCell.position);
            Map.mouseCursor.UpdatePosition(MovingEndCell.position);
            SelectedUnit.role.OnMoveEnd(endCell.g); //减去移动消耗
            ShowMapMenu(true);
        }


        /// <summary>
        /// 攻击
        /// </summary>
        /// <param name="mapClass"></param>
        /// <param name="target"></param>
        protected void AttackMapClass(MapClass mapClass, MapClass target)
        {
            //停止移动动画 ，并隐藏光标
            mapClass.animatorController.StopMove();
            Map.HideRangeCursors();
            DisplayMouseCursor(false);
            // 计算战斗并开始战斗动画
            CombatAnimaController combatAnim = Combat.GetOrAdd(Map.gameObject);
            combatAnim.LoadCombatUnit(mapClass, target);
            combatAnim.onPlay.AddListener(MapClass_OnCombatAnimaPlay);
            combatAnim.onStep.AddListener(MapClass_OnCombatAnimaStep);
            combatAnim.onStop.AddListener(MapClass_OnCombatAnimaStop);

            TriggerEvents(MapEventConditionType.RoleCombatTalkCondition, () =>
            {
                MapStatus = MapStatus.Animation;
                combatAnim.PlayAnimas(true);
            });
        }

        private void MapClass_OnCombatAnimaPlay(CombatAnimaController combatAnima, bool inMap)
        {
            // TODO 打开UI面板，进行初步设置
        }

        private void MapClass_OnCombatAnimaStop(CombatAnimaController combatAnima, bool inMap)
        {
            combatAnima.onPlay.RemoveListener(MapClass_OnCombatAnimaPlay);
            combatAnima.onStep.RemoveListener(MapClass_OnCombatAnimaStep);
            combatAnima.onStop.RemoveListener(MapClass_OnCombatAnimaStop);

            MapClass unit0 = combatAnima.combat.GetCombatUnit(0).mapClass;
            MapClass unit1 = combatAnima.combat.GetCombatUnit(1).mapClass;
            unit0.animatorController.StopPrepareAttack();
            unit1.animatorController.StopPrepareAttack();
            combatAnima.combat.BattleEnd();

            if (unit0.role.isDead)
            {
                TriggerEvents(MapEventConditionType.RoleDeadCondition, () =>
                {
                    ClearSelected();
                    OnMapClassDead(unit0);
                    //TODO 关闭界面
                });
            }
            else if (unit1.role.isDead)
            {
                TriggerEvents(MapEventConditionType.RoleDeadCondition, () =>
                {
                    HoldingMapClass(unit0);
                    OnMapClassDead(unit1);
                    //TODO 关闭界面
                });
            }
            else
            {
                HoldingMapClass(unit0);
                //TODO 关闭界面
            }
        }

        protected virtual void ClearSelected()
        {
            if (SelectedUnit != null)
            {
                SelectedUnit.animatorController.StopMove();
                SelectedUnit.animatorController.StopPrepareAttack();
            }

            if (TargetUnit != null)
            {
                TargetUnit.animatorController.StopMove();
                TargetUnit.animatorController.StopPrepareAttack();
            }

            SelectedCell = null;
            SelectedUnit = null;
            MovingEndCell = null;
            TargetUnit = null;
            MapStatus = MapStatus.Normal;
            DisplayMouseCursor(true);
        }

        private void OnMapClassDead(MapClass mapClass)
        {
            CellData cellData = Map.GetCellData(mapClass.cellPosition);
            cellData.mapObject = null;
            m_UnitDict[mapClass.role.attitudeTowards].Remove(mapClass);
            if (m_UnitDict[mapClass.role.attitudeTowards].Count == 0)
            {
                m_UnitDict.Remove(mapClass.role.attitudeTowards);
            }
            //TODO 回池 mapClass.gameObject
        }

        /// <summary>
        /// 获取非待机颜色
        /// </summary>
        /// <param name="attitudeTowards"></param>
        /// <returns></returns>
        public string GetSwapperColorName(AttitudeTowards attitudeTowards)
        {
            switch (attitudeTowards)
            {
                case AttitudeTowards.Player:
                    return "ClassBlue";
                case AttitudeTowards.Enemy:
                    return "ClassRed";
                case AttitudeTowards.Ally:
                    return "ClassGreen";
                case AttitudeTowards.Neutral:
                    return "ClassYellow";
                default:
                    return "ClassRed";
            }
        }

        /// <summary>
        /// 获取待机颜色
        /// </summary>
        /// <returns></returns>
        public string GetSwapperHoldingColorName()
        {
            return "ClassGray";
        }

        protected void HoldingMapClass(MapClass mapClass)
        {
            //只有玩家才会设置待机状态和颜色
            if (mapClass.role.attitudeTowards == AttitudeTowards.Player)
            {
                mapClass.role.Holding(true);
                mapClass.swapper.SwapColors(GetSwapperHoldingColorName());
            }

            // 如果是独有角色，触发坐标事件
            if (mapClass.role.roleType == RoleType.Unique)
            {
                TriggerEvents(MapEventConditionType.PositionCondition, ClearSelected);
            }
            else
            {
                ClearSelected();
            }
        }

        private void MapClass_OnCombatAnimaStep(CombatAnimaController combatAnima, int index, float wait, bool end)
        {
            // 每一步更新生命等属性
            if (end)
            {
                return;
            }

            CombatStep step = combatAnima.combat.steps[index];
            //如果是准备阶段，打开面板中的战斗传酷狗，否则刷新血量数据
            if (step.atkVal.animaType == CombatAnimaType.Prepare)
            {
                MapClass leftUnit = combatAnima.combat.GetCombatUnit(0).mapClass;
                MapClass rightUnit = combatAnima.combat.GetCombatUnit(1).mapClass;
                UIBattleUnitInMap.RoleArg left = new UIBattleUnitInMap.RoleArg()
                {
                    roleType = leftUnit.role.roleType,
                    id = rightUnit.role.characterId,
                    guid = rightUnit.role.guid
                };
                //TODO 打开界面 UIBattleInMapPanel
                // panel.OpenBattleWindow(left,right,wait)
            }
            else
            {
                //TODO 更新界面血量
                // panel.UpdateHp(step.GetCombatVariable(0).hp, step.GetCombatVariable(1).hp)
            }
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        /// <param name="result"></param>
        public void MapEndCommand(int result)
        {
            //TODO 游戏结算 处理
            // if (m_EventCoroutine != null)
            // {
            //     GameEntry.GameDirector.StopCoroutine(m_EventCoroutine);
            //     m_EventCoroutine = null;
            // }
            //
            // GameEntry.GameDirector.StopGameAction();
            // if (previous != null)
            // {
            //     ScenarioBlackboard.Set(ScenarioBlackboard.battleMapScene, result);
            //     (previous as ScenarioAction).BattleMapDone(error);
            //     GameEntry.GameDirector.BackGameAction();
            //     MessageCenter.AddListener(GameMain.k_Event_OnSceneLoaded, MapEndCommand_OnSceneLoaded);
            //     if (string.IsNullOrEmpty(nextScene))
            //     {
            //         GameMain.instance.LoadSceneAsync(ScenarioBlackboard.lastScenarioScene,
            //             mode: LoadSceneMode.Additive);
            //     }
            //     else
            //     {
            //         GameMain.instance.LoadSceneAsync(nextScene, mode: LoadSceneMode.Additive);
            //     }
            // }
            // else
            // {
            //     Debug.LogError("MapAction -> no previous game action.");
            //     GameDirector.instance.BackGameAction();
            // }
        }

        private void MapEndCommand_OnSceneLoaded(string message, object sender, /*MessageArgs messageArgs,*/
            params object[] messageParams)
        {
            // MessageCenter.RemoveListener(GameMain.k_Event_OnSceneLoaded, MapEndCommand_OnSceneLoaded);
            //
            // OnSceneLoadedArgs args = messageArgs as OnSceneLoadedArgs;
            // GameMain.instance.SetActiveScene(args.scene.name);
            // GameMain.instance.UnloadSceneAsync(ScenarioBlackboard.battleMapScene);
            // ScenarioBlackboard.battleMapScene = string.Empty;
            // ScenarioBlackboard.mapScript = string.Empty;
            //
            // GameDirector.instance.RunGameAction();
        }

        /// <summary>
        /// 每帧刷新
        /// </summary>
        /// <returns></returns>
        public override bool Update()
        {
            //如果是剧本，就运行剧本
            if (Status == ActionStatus.WaitScenarioDone)
            {
                if (!ScenarioAction.Update())
                {
                    if (ScenarioAction.status == ActionStatus.Error)
                    {
                        Status = ActionStatus.Error;
                        error = ScenarioAction.error;
                        Abort();
                    }
                    
                    if (Turn != AttitudeTowards.Player)
                    {
                        if (MapStatus == MapStatus.Animation 
                            || MapStatus == MapStatus.Event 
                            || MapStatus == MapStatus.Menu || MapStatus == MapStatus.SubMenu)
                        {
                            return true;
                        }

                        // TODO 具体NPC操作
                        //获取NPC
                        List<MapClass> units;
                        if (!m_UnitDict.TryGetValue(Turn,out  units) || m_NpcIndex >= units.Count)
                        {
                            NextTurn();
                            return true;
                        }

                        MapClass unit = units[m_NpcIndex];
                        //光标跟随
                        if (Map.mouseCursor.cellPosition  != unit.cellPosition)
                        {
                            MoveCursorCommand(unit.cellPosition, 0.5f);
                            return true;
                        }

                        //NPC 移动
                        if (MapStatus != MapStatus.AttackCursor)
                        {
                            NpcMove(unit);
                        }
                        else //NPC 攻击
                        {
                            NpcAttack(unit);
                        }


                        return true;
                    }
                    return false;
                }
            }

            return true;
        }
        
        
        /// <summary>
        /// 移动光标
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="time"></param>
        public void MoveCursorCommand(Vector3Int targetPos, float time, Action onMoveEnd = null)
        {
            if (time <= 0f)
            {
                Map.mouseCursor.UpdatePosition(targetPos);
                if (onMoveEnd != null)
                {
                    onMoveEnd();
                }
            }
            GameEntry.Instance.StartCoroutine(MovingCursorToCell(targetPos, time, onMoveEnd));
        }

        private IEnumerator MovingCursorToCell(Vector3Int targetPos, float time, Action onMoveEnd)
        {
            MapStatus = MapStatus.Animation;

            Vector3 start = Map.mouseCursor.transform.position;
            Vector3 end = Map.GetCellPosition(targetPos);
            float t = 0f;

            while (t < time)
            {
                t += Time.deltaTime;
                Vector3 pos = Vector3.Lerp(start, end, t / time);
                Map.mouseCursor.transform.position = pos;
                yield return null;
            }

            Map.mouseCursor.UpdatePosition(targetPos);
            
            // 最后延迟0.5秒
            yield return new WaitForSeconds(0.5f);
            MapStatus = MapStatus.Normal;
            if (onMoveEnd != null)
            {
                onMoveEnd();
            }
        }
        

        /// <summary>
        /// 判断是否可以输入
        /// </summary>
        /// <param name="cancelButton"></param>
        /// <returns></returns>
        public virtual bool CanInput(bool cancelButton)
        {
            if (Status != ActionStatus.WaitInput || Turn != AttitudeTowards.Player)
            {
                return false;
            }

            if (MapStatus == MapStatus.Animation || MapStatus == MapStatus.Event)
            {
                return false;
            }

            if (!cancelButton)
            {
                if (MapStatus == MapStatus.Menu || MapStatus == MapStatus.SubMenu)
                {
                    return false;
                }
            }

            return true;
        }

        protected Vector3Int MouseToCellPosition(Vector3 mousePosition)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3Int cellPosition = Map.grid.WorldToCell(worldPosition);
            return cellPosition;
        }

        public override void OnMouseMove(Vector3 mousePosition)
        {
            if (Status == ActionStatus.WaitScenarioDone)
            {
                ScenarioAction.OnMouseMove(mousePosition);
                return;
            }

            if (!CanInput(false))
            {
                return;
            }

            Vector3Int cellPosition = MouseToCellPosition(mousePosition);
            Map.mouseCursor.UpdatePosition(cellPosition);
        }

        public override void OnMouseRButtonDown(Vector3 mousePosition)
        {
            if (Status == ActionStatus.WaitScenarioDone)
            {
                ScenarioAction.OnMouseRButtonDown(mousePosition);
                return;
            }

            if (!CanInput(true))
            {
                return;
            }

            switch (MapStatus)
            {
                case MapStatus.Menu:
                case MapStatus.SubMenu:
                    //TODO UI 关闭子菜单
                    ResetSelected();
                    break;
                case MapStatus.MoveCursor:
                    ResetSelected();
                    break;
                case MapStatus.AttackCursor:
                    Map.HideRangeCursors();
                    DisplayMouseCursor(false);
                    ShowMapMenu(true);
                    break;
            }
        }

        public void DisplayMouseCursor(bool show)
        {
            if (show)
            {
                //只有玩家才会显示
                if (Turn == AttitudeTowards.Player)
                {
                    Map.mouseCursor.DisPlayCursor(true);
                }
            }
            else
            {
                Map.mouseCursor.DisPlayCursor(false);
            } 
        }

        protected virtual void ResetSelected()
        {
            //如果选择了目标
            if (SelectedUnit != null)
            {
                //隐藏移动和攻击光标
                Map.HideRangeCursors();
                //如果移动过了
                if (MovingEndCell != null)
                {
                    //重置格子数据
                    MovingEndCell.mapObject = null;
                    SelectedCell.mapObject = SelectedUnit;
                    MovingEndCell = null;
                    //重置角色位置和移动力
                    SelectedUnit.UpdatePosition(SelectedCell.position);
                    SelectedUnit.role.ResetMovePoint();

                    //停止动画
                    SelectedUnit.animatorController.StopMove();
                    SelectedUnit.animatorController.StopPrepareAttack();
                }

                SelectedCell = null;
                SelectedUnit = null;
            }

            MapStatus = MapStatus.Normal;
            DisplayMouseCursor(true);
        }
        
        protected Coroutine m_EventCoroutine = null;
        
        private int m_NpcIndex = 0;

        /// <summary>
        /// 触发地图事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="onTriggerEnd"></param>
        public void TriggerEvents(MapEventConditionType type, Action onTriggerEnd)
        {
            MapStatus = MapStatus.Event;
            if (m_EventCoroutine == null)
            {
                error = "MapAction TriggerEvents -> event is running";
                Status = ActionStatus.Error;
                Debug.LogError(error);
                if (onTriggerEnd != null)
                {
                    onTriggerEnd();
                }

                return;
            }

            m_EventCoroutine = GameEntry.Instance.StartCoroutine(EventsTriggering(type, onTriggerEnd));
        }

        private IEnumerator EventsTriggering(MapEventConditionType type, Action onTriggerEnd)
        {
            yield return null;
            switch (type)
            {
                case MapEventConditionType.NoneCondition:
                    yield return m_MapEvents.TriggerStartEvents(
                        this, onError: eventError => error = eventError);
                    break;
                case MapEventConditionType.TurnCondition:
                    yield return m_MapEvents.TriggerTurnEvents(
                        this, onError: eventError => error = eventError);
                    break;
                case MapEventConditionType.PositionCondition:
                    if (MovingEndCell != null)
                    {
                        yield return m_MapEvents.TriggerPositionEvents(
                            this, MovingEndCell.position, onError: eventError => error = eventError);
                    }

                    break;
                case MapEventConditionType.RoleDeadCondition:
                    if (SelectedUnit != null && SelectedUnit.role.isDead)
                    {
                        yield return m_MapEvents.TriggerDeadEvents(
                            this, SelectedUnit.role.characterId, onError: eventError => error = eventError);
                    }

                    if (TargetUnit != null && TargetUnit.role.isDead)
                    {
                        yield return m_MapEvents.TriggerDeadEvents(
                            this, TargetUnit.role.characterId, onError: eventError => error = eventError);
                    }

                    break;
                case MapEventConditionType.RoleTalkCondition:
                    if (SelectedUnit != null && TargetUnit != null)
                    {
                        yield return m_MapEvents.TriggerRoleTalkEvents(
                            this, SelectedUnit.role.characterId, TargetUnit.role.characterId,
                            onError: eventError => error = eventError);
                    }

                    break;
                case MapEventConditionType.RoleCombatTalkCondition:
                    if (SelectedUnit != null && TargetUnit != null)
                    {
                        yield return m_MapEvents.TriggerRoleCombatTalkEvents(
                            this, SelectedUnit.role.characterId, TargetUnit.role.characterId,
                            onError: eventError => error = eventError);
                    }

                    break;
            }

            m_EventCoroutine = null;
            if (onTriggerEnd != null)
            {
                onTriggerEnd();
            }
        }

        /// <summary>
        /// NPC 移动
        /// </summary>
        /// <param name="npc"></param>
        public void NpcMove(MapClass npc)
        {
            Vector3Int npcPosition = npc.cellPosition;
            CellData npcCell = Map.GetCellData(npcPosition);
            SelectedCell = npcCell;
            SelectedUnit = npc;
            
            //TODO 之类可以根据AI 来判断敌人的一共
            //NpcAi ai = ai.Model.Get(npc.id);
            //以下是寻找敌人进行攻击的建议AI(忽略了治疗或状态等的移动)
            
            //假设中立不对不可移动
            if (Turn == AttitudeTowards.Neutral)
            {
                ClearSelected();
                m_NpcIndex++;
                return;
            }
            
            CellData moveToCell = npcCell;
            //TODO 简易AI ，朝向最近目标移动或攻击
            //移动
            MoveMapClass(npc, moveToCell);
            
            //TODO 以下是简易 搜寻可攻击目标
            HashSet<CellData> moveRange = new HashSet<CellData>(
                Map.SearchMoveRange(npcCell, npc.role.movePoint, npc.role.moveConsumption));

            bool atk = false; // 是否有可攻击的目标
            for (int i = 0; i < npc.role.items.Length; i++)
            {
                Item item = npc.role.items[i];
                if (item == null || item.ItemType != ItemType.Weapon)
                {
                    continue;
                }
                
                //搜索所有移动范围内的可攻击目标
                Dictionary<CellData, HashSet<CellData>> deferDict =
                    new Dictionary<CellData, HashSet<CellData>>(Map.cellPositionEqualityCompaper);
                
                foreach (CellData cell in moveRange)
                {
                    List<CellData> atkCells = Map.SearchAttackRange(cell, (item as Weapon).minRange,
                        (item as Weapon).maxRange, true);
                    //防守者
                    // 1 必须是有地图对象
                    // 2 地图对象是地图职业
                    // 3 目标不能同阵营（如果是治疗，必须是同阵营或同盟）
                    // 4 目标不能是中立
                    IEnumerable<CellData> defers = atkCells.Where(c => c.hasMapObject &&
                                                                       c.mapObject.mapObjectType ==
                                                                       MapObjectType.Class &&
                                                                       (c.mapObject as MapClass).role.attitudeTowards !=
                                                                       Turn &&
                                                                       (c.mapObject as MapClass).role.attitudeTowards !=
                                                                       AttitudeTowards.Neutral);
                    //如果是盟友，还不应包括玩家
                    if (npc.role.attitudeTowards == AttitudeTowards.Ally)
                    {
                        defers = defers.Where(c =>
                            (c.mapObject as MapClass).role.attitudeTowards != AttitudeTowards.Player);
                    }

                    foreach (CellData c in defers)
                    {
                        if (!deferDict.ContainsKey(c))
                        {
                            deferDict[c] = new HashSet<CellData>(Map.cellPositionEqualityCompaper);
                        }
                        deferDict[c].Add(cell);
                    }
                }

                if (deferDict.Count > 0)
                {
                    //寻找最近的目标
                    CellData targetCell = null;
                    int minDist = int.MaxValue;
                    foreach (CellData cell in deferDict.Keys)
                    {
                        int dist = CalcCellDist(cell.position, npcPosition);
                        if (dist < minDist)
                        {
                            targetCell = cell;
                            minDist = dist;
                        }
                    }
                    //寻找最近的可攻击到目标的位置
                    minDist = int.MaxValue;
                    foreach (CellData cell in deferDict[targetCell])
                    {
                        int dist = CalcCellDist(cell.position, npcPosition);
                        if (dist < minDist)
                        {
                            moveToCell = cell;
                            minDist = dist;
                        }
                    }
                    
                    //设置攻击目标
                    TargetUnit = targetCell.mapObject as MapClass;
                    ;
                    atk = true;
                    npc.role.EquipWeapon(item as Weapon);
                }

                // 如果能攻击到目标
                if (atk)
                {
                    break;
                }
            }
        }

        private int CalcCellDist(Vector3Int cellPosition, Vector3Int npcPosition)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// NPC 攻击
        /// </summary>
        /// <param name="npc"></param>
        public void NpcAttack(MapClass npc)
        {
            //TODO
        }

        /// <summary>
        /// 转换回合
        /// </summary>
        protected void NextTurn()
        {
            //重置npc下标
            m_NpcIndex = 0;
            switch (Turn)
            {
                case AttitudeTowards.Player:
                    TurnToken++; // 只有所有阵营结束，回合数才+1

                    // 将所有玩家回复移动力，并将待机状态重置
                    string color = GetSwapperColorName(AttitudeTowards.Player);
                    foreach (MapClass unit in m_UnitDict[AttitudeTowards.Player])
                    {
                        unit.role.Holding(false);
                        unit.role.ResetMovePoint();
                        unit.swapper.SwapColors(color);
                    }

                    // 隐藏光标
                    DisplayMouseCursor(false);
                    Turn = AttitudeTowards.Enemy;
                    GameEntry.GameDirector.RunGameAction();
                    break;
                case AttitudeTowards.Enemy:
                    Turn = AttitudeTowards.Ally;
                    break;
                case AttitudeTowards.Ally:
                    Turn = AttitudeTowards.Neutral;
                    break;
                case AttitudeTowards.Neutral:
                    Turn = AttitudeTowards.Player;
                    break;
            }


            if (m_UnitDict.ContainsKey(Turn))
            {
                OnChangeTurn(Turn);
            }
            else
            {
                // 如果不存在MapClass，直接下一组回合
                NextTurn();
            }
        }

        private void OnChangeTurn(AttitudeTowards turn)
        {
            // 播放转换回合的UI动画
            MapStatus = MapStatus.Animation;
            //TODO UI 相关处理
            // UIChangeTurnPanel panel = UIManager.views.OpenView<UIChangeTurnPanel>(UINames.k_UIChangeTurnPanel, false);
            // panel.ChangeTurn(turn, () =>
            // {
            //     UIManager.views.CloseView();
            //
            //     // 触发回合事件
            //     TriggerEvents(MapEventConditionType.TurnCondition, () =>
            //     {
            //         if (turn == AttitudeTowards.Player)
            //         {
            //             // 移动光标到首个玩家单位
            //             MoveCursorCommand(m_UnitDict[AttitudeTowards.Player][0].cellPosition, 0.5f, ClearSelected);
            //         }
            //         else
            //         {
            //             ClearSelected();
            //         }
            //     });
            // });
        }
        
        /// <summary>
        /// 读取事件
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool LoadMapEvent(MapEventInfo info)
        {
            // 地图加载事件
            if (info.startEvent != null)
            {
                if (info.startEvent.entryConditionType != MapEventConditionType.NoneCondition)
                {
                    info.startEvent.entryConditionType = MapEventConditionType.NoneCondition;
                }
                m_MapEvents.Add(info.startEvent);
            }

            // 地图中的事件
            if (info.events != null)
            {
                for (int i = 0; i < info.events.Length; i++)
                {
                    MapEvent me = info.events[i];
                    if (me == null)
                    {
                        continue;
                    }

                    if (!m_MapEvents.Add(me))
                    {
                        error = string.Format(
                            "MapAction -> Load map event failure. Entry type `{0}` is not supported."
                            + " Or, id of map event is already exist.",
                            me.entryConditionType);
                        return false;
                    }
                }
            }

            // 地图结束事件
            if (info.resultEvents != null)
            {
                for (int i = 0; i < info.resultEvents.Length; i++)
                {
                    MapEvent me = info.resultEvents[i];
                    if (me == null)
                    {
                        continue;
                    }

                    if (!m_MapEvents.Add(me))
                    {
                        error = string.Format(
                            "MapAction -> Load map event failure. Entry type `{0}` is not supported."
                            + " Or, id of map event is already exist.",
                            me.entryConditionType);
                        return false;
                    }
                }
            }

            // 执行地图加载事件
            TriggerEvents(MapEventConditionType.NoneCondition, () =>
            {
                OnChangeTurn(AttitudeTowards.Player);
            });

            return true;
        }
        
        
        
        public override void Dispose()
        {
            base.Dispose();

            Status = ActionStatus.Error;
            if (ScenarioAction != null)
            {
                ScenarioAction.Dispose();
                ScenarioAction = null;
            }
            Turn = AttitudeTowards.Player;
            TurnToken = 0;

            SelectedCell = null;
            SelectedUnit = null;
            MapStatus = MapStatus.Normal;
            MovingEndCell = null;
            TargetUnit = null;

            //npcIndex = 0;

            foreach (KeyValuePair<AttitudeTowards, List<MapClass>> kvp in m_UnitDict)
            {
                if (kvp.Value == null)
                {
                    continue;
                }

                foreach (MapClass unit in kvp.Value)
                {
                    //ObjectPool.DespawnUnsafe(unit.gameObject, true);
                }
            }
            m_UnitDict.Clear();

            m_NpcIndex = 0;
            
            foreach (MapObstacle obstacle in m_Obstacles)
            {
                //ObjectPool.DespawnUnsafe(obstacle.gameObject, true);
            }
            m_Obstacles.Clear();

            if (m_EventCoroutine != null)
            {
                GameEntry.Instance.StopCoroutine(m_EventCoroutine);
                m_EventCoroutine = null;
            }
            m_MapEvents?.Clear();

            if (Map != null)
            {
                Map.HideRangeCursors();
                //ObjectPool.DespawnUnsafe(m_Map.mouseCursor.gameObject);
                //map.mapCursorPool.DestroyRecycledImmediate();
                //map.mapObjectPool.DestroyRecycledImmediate();
                Map = null;
            }
        }
        
        
    }
}