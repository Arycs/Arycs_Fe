using System;
using System.Collections;
using System.Collections.Generic;
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
        public MapGraph map { get; set; }
        private string nextScene { get; set; } = string.Empty;
        public ActionStatus status { get; set; } = ActionStatus.Error;
        public MapScenarioAction scenarioAction = null;

        public MapStatus mapStatus { get; set; } = MapStatus.Normal;

        public AttitudeTowards turn { get; set; } = AttitudeTowards.Player;
        public int turnToken { get; set; } = 0;

        public CellData selectedCell { get; set; } = null;
        public MapClass selectedUnit { get; set; } = null;
        public MapClass targetUnit { get; set; } = null;
        private CellData movingEndCell { get; set; } = null;

        protected readonly Dictionary<AttitudeTowards, List<MapClass>> m_UnitDict =
            new Dictionary<AttitudeTowards, List<MapClass>>();

        protected readonly HashSet<MapObstacle> m_Obstacles = new HashSet<MapObstacle>();

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
            map = GameObject.FindObjectOfType<MapGraph>();
            if (map == null)
            {
                error = "MapAction -> MapGraph was not found";
                return false;
            }

            map.InitMap();
            //TODO 读取地图脚本，加载资源,根据 scriptName 来加载地图脚本
            TextAsset asset = new TextAsset();
            if (asset == null)
            {
                error = "MapAction -> script file was not found";
                return false;
            }

            //TODO 根据加载的内容，解析地图事件信息
            MapEventInfo info = new MapEventInfo();
            if (info == null)
            {
                error = "MapAction -> Load map event info from xml bytes failure";
                return false;
            }

            //地图结束后的场景名称 可以为null
            // 为null时， 回到上一个场景
            nextScene = info.nextScene;

            //设置鼠标光标
            if (!string.IsNullOrEmpty(info.mouseCursor.prefab))
            {
                //TODO  加载资源获取鼠标光标
                MapMouseCursor mouseCursor = new MapMouseCursor();
                if (mouseCursor != null)
                {
                    map.mouseCursorPrefab = mouseCursor;
                }
            }

            map.mouseCursor.UpdatePosition(new Vector3Int(info.mouseCursor.x, info.mouseCursor.y, 0));

            //设置移动范围和攻击范围的图标
            if (!string.IsNullOrWhiteSpace(info.cursor))
            {
                //TODO 加载资源后去移动/攻击光标
                MapCursor cursor = new MapCursor();
                if (cursor != null)
                {
                    map.cursorPrefab = cursor;
                }
            }

            //读取剧情剧本与地图事件
            if (!LoadScenario(info.scenarioName) || !LoadMapEvent(info))
            {
                return false;
            }

            //设置状态
            status = ActionStatus.WaitInput;
            return true;
        }

        private bool LoadMapEvent(MapEventInfo info)
        {
            throw new System.NotImplementedException();
        }

        private bool LoadScenario(string scriptName)
        {
            //剧本可以为null ，地图中没有剧情，纯战斗
            if (string.IsNullOrEmpty(scriptName))
            {
                return true;
            }

            //TODO  根据scriptName 获取Action
            Iscenario scenario = new TxtScript();
            if (scenario == null)
            {
                error = $"MapAction -> LoadScenario error .Script '{scriptName}' was not fount";
                return false;
            }

            MapScenarioAction action = new MapScenarioAction(this);
            List<Type> executorTypes = GameAction.GetDefaultExecutorTypesForScenarioAction();
            action.LoadExecutors(executorTypes.ToArray());
            if (!action.LoadScenario(scenario))
            {
                error = action.error;
                action.Dispose();
                return false;
            }

            scenarioAction = action;
            return true;
        }

        public void ScenarioDone()
        {
            if (status == ActionStatus.WaitScenarioDone)
            {
                status = ActionStatus.WaitInput;
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
            if (scenarioAction == null)
            {
                error = "MapAction -> No Scenario";
                return false;
            }

            ActionStatus s = scenarioAction.GotoCommand(flag, out m_Error);
            if (s == ActionStatus.Error)
            {
                status = ActionStatus.Error;
                return false;
            }

            status = ActionStatus.WaitScenarioDone;
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
            CellData cellData = map.GetCellData(position);
            if (cellData == null)
            {
                cmdError =
                    $"{"ObjectExecutor"} Create Obstacle -> position '{position.ToString()}' is out of range, prefab : {prefab} ";
                return ActionStatus.Error;
            }

            if (cellData.hasMapObject)
            {
                cmdError =
                    $"{"ObjectExecutor"} Create Obstacle -> the object in position '{position.ToString()}' is already, prefab :{prefab}";
                return ActionStatus.Error;
            }

            MapObject mapObject = map.CreateMapObject(prefab, position);
            if (mapObject == null || mapObject.mapObjectType != MapObjectType.Obstacle)
            {
                cmdError = $"{"ObjectExecutor"} Create Obstacle -> create map object error. prefab :{prefab} ";
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

        public ActionStatus ObjectCommandCreateClass(AttitudeTowards attitudeTowards, RoleType roleType, int id,
            Vector3Int position, int level, int[] items, out string cmdError)
        {
            if (roleType == RoleType.Following && attitudeTowards == AttitudeTowards.Player)
            {
                cmdError = $"{"ObjectExecutor"} Create Class -> role type of player can only be 'Unique'";
                return ActionStatus.Error;
            }

            CellData cellData = map.GetCellData(position);
            if (cellData == null)
            {
                cmdError =
                    $"{"ObjectExecutor"} Create Class -> position '{position.ToString()}',id :{id.ToString()}";
                return ActionStatus.Error;
            }

            if (cellData.hasMapObject)
            {
                cmdError =
                    $"{"ObjectExecutor"} Create Class -> the object in position '{position.ToString()}' is already exist. id :{id.ToString()}";
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


            MapObject mapObject = map.CreateMapObject(prefab, position);
            if (mapObject == null)
            {
                cmdError = $"{"ObjectExecutor"} Create Class -> create map object error";
                return ActionStatus.Error;
            }

            if (mapObject.mapObjectType != MapObjectType.Class)
            {
                cmdError =
                    $"{"ObjectExecutor"} Create Class -> Create map object error, type error, id:{id.ToString()}";
                //TODO 回池
                //ObjectPool.DespawnUnsafe(mapObject.gameObject, true);
                return ActionStatus.Error;
            }

            MapClass mapCls = mapObject as MapClass;
            if (!mapCls.Load(id, roleType))
            {
                cmdError =
                    $"{"ObjectExecutor"} Run -> load role error. id :{id.ToString()}, role type :{roleType.ToString()}";
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

        public override void OnMouseLButtonDown(Vector3 mousePosition)
        {
            if (status == ActionStatus.WaitScenarioDone)
            {
                scenarioAction.OnMouseLButtonDown(mousePosition);
                return;
            }

            if (!CanInput(false))
            {
                return;
            }

            //获取点击的CellData
            Vector3Int cellPosition = map.mouseCursor.cellPosition;
            CellData cell = map.GetCellData(cellPosition);
            if (cell == null)
            {
                return;
            }

            switch (mapStatus)
            {
                case MapStatus.Normal:
                    if (selectedUnit == null)
                    {
                        // 如果Cell中有角色
                        if (cell.hasMapObject && cell.mapObject.mapObjectType == MapObjectType.Class)
                        {
                            MapClass mapClass = cell.mapObject as MapClass;
                            selectedCell = cell;
                            selectedUnit = mapClass;
                        }
                    }

                    ShowMapMenu(false);
                    break;
                case MapStatus.MoveCursor:
                    if (selectedUnit.role.attitudeTowards != AttitudeTowards.Player)
                    {
                        map.HideRangeCursors();
                        selectedUnit = null;
                        selectedCell = null;
                        mapStatus = MapStatus.Normal;
                        break;
                    }

                    //如果选中的不在移动范围内
                    if (!cell.hasMoveCursor)
                    {
                        break;
                    }

                    //如果选中的在移动范围内，那么开始移动，并播放移动动画
                    MoveMapClass(selectedUnit, cell);
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

                    targetUnit = target;
                    //攻击目标
                    AttackMapClass(selectedUnit, target);
                    break;
            }
        }

        /// <summary>
        /// 显示地图菜单
        /// </summary>
        /// <param name="sub"></param>
        protected void ShowMapMenu(bool sub)
        {
            //TODO 打开MapMenu 界面
            HashSet<MenuTextID> showButtons;

            // 如果点击的不是角色，那么显示主菜单
            if (selectedUnit == null)
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
                if (selectedUnit.role.attitudeTowards == AttitudeTowards.Player)
                {
                    //如果是移动后的菜单
                    if (sub)
                    {
                        showButtons.Remove(MenuTextID.Move);
                        showButtons.Remove(MenuTextID.Status);
                        //如果物品栏里没有武器， 或所有武器不可用
                        bool canAtk = false;
                        for (int i = 0; i < selectedUnit.role.items.Length; i++)
                        {
                            Item item = selectedUnit.role.items[i];
                            if (item == null || item.ItemType != ItemType.Weapon)
                            {
                                continue;
                            }

                            //有武器，但是职业不能装备此武器
                            if ((item as Weapon).level >
                                selectedUnit.role.cls.info.AvailableWeapons[(item as Weapon).weaponType])
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
                        if (selectedUnit.role.holding)
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
                    if (map.SearchAndShowMoveRange(selectedUnit, true))
                    {
                        mapStatus = MapStatus.MoveCursor;
                        DisplayMouseCursor(true);
                    }
                    else
                    {
                        mapStatus = MapStatus.Normal;
                        error = "MapAction -> Search Move Range error";
                        status = ActionStatus.Error;
                    }

                    break;
                case MenuTextID.Attack:
                    //TODO 这里应该显示选择武器面板，后续修改
                    Weapon roleWeapon = selectedUnit.role.equipedWeapon;
                    List<CellData> atkCells =
                        map.SearchAttackRange(movingEndCell, roleWeapon.minRange, roleWeapon.maxRange, true);
                    map.ShowRangeCursors(atkCells, MapCursor.CursorType.Attack);
                    mapStatus = MapStatus.AttackCursor;
                    DisplayMouseCursor(true);
                    break;
                case MenuTextID.Holding:
                    HoldingMapClass(selectedUnit);
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
            map.HideRangeCursors();
            DisplayMouseCursor(false);
            // 开始移动动画
            Stack<CellData> path = map.searchPath.BuildPath(moveTo);
            mapClass.onMovingEnd += MapClass_OnMovingEnd;
            mapStatus = MapStatus.Animation;
            mapClass.animatorController.PlayMove();
            mapClass.StartMove(path);
        }

        private void MapClass_OnMovingEnd(CellData endCell)
        {
            selectedUnit.onMovingEnd -= MapClass_OnMovingEnd;

            //设置坐标
            selectedCell.mapObject = null;
            movingEndCell = endCell;
            movingEndCell.mapObject = selectedUnit;
            selectedUnit.UpdatePosition(movingEndCell.position);
            map.mouseCursor.UpdatePosition(movingEndCell.position);
            selectedUnit.role.OnMoveEnd(endCell.g); //减去移动消耗
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
            map.HideRangeCursors();
            DisplayMouseCursor(false);
            // 计算战斗并开始战斗动画
            CombatAnimaController combatAnim = Combat.GetOrAdd(map.gameObject);
            combatAnim.LoadCombatUnit(mapClass, target);
            combatAnim.onPlay.AddListener(MapClass_OnCombatAnimaPlay);
            combatAnim.onStep.AddListener(MapClass_OnCombatAnimaStep);
            combatAnim.onStop.AddListener(MapClass_OnCombatAnimaStop);
            mapStatus = MapStatus.Animation;
            combatAnim.PlayAnimas(true);
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
                ClearSelected();
                OnMapClassDead(unit0);
                //TODO 关闭界面
            }
            else if (unit1.role.isDead)
            {
                HoldingMapClass(unit0);
                OnMapClassDead(unit1);
                //TODO 关闭界面
            }
            else
            {
                HoldingMapClass(unit0);
                //TODO 关闭界面
            }
        }

        protected virtual void ClearSelected()
        {
            if (selectedUnit != null)
            {
                selectedUnit.animatorController.StopMove();
                selectedUnit.animatorController.StopPrepareAttack();
            }

            if (targetUnit != null)
            {
                targetUnit.animatorController.StopMove();
                targetUnit.animatorController.StopPrepareAttack();
            }

            selectedCell = null;
            selectedUnit = null;
            movingEndCell = null;
            targetUnit = null;
            mapStatus = MapStatus.Normal;
            DisplayMouseCursor(true);
        }

        private void OnMapClassDead(MapClass mapClass)
        {
            CellData cellData = map.GetCellData(mapClass.cellPosition);
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

            ClearSelected();
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
            if (status == ActionStatus.WaitScenarioDone)
            {
                if (!scenarioAction.Update())
                {
                    if (scenarioAction.status == ActionStatus.Error)
                    {
                        status = ActionStatus.Error;
                        error = scenarioAction.error;
                        Abort();
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断是否可以输入
        /// </summary>
        /// <param name="cancelButton"></param>
        /// <returns></returns>
        public virtual bool CanInput(bool cancelButton)
        {
            if (status != ActionStatus.WaitInput || turn != AttitudeTowards.Player)
            {
                return false;
            }

            if (mapStatus == MapStatus.Animation || mapStatus == MapStatus.Event)
            {
                return false;
            }

            if (!cancelButton)
            {
                if (mapStatus == MapStatus.Menu || mapStatus == MapStatus.SubMenu)
                {
                    return false;
                }
            }

            return true;
        }

        protected Vector3Int MouseToCellPosition(Vector3 mousePosition)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3Int cellPosition = map.grid.WorldToCell(worldPosition);
            return cellPosition;
        }

        public override void OnMouseMove(Vector3 mousePosition)
        {
            if (status == ActionStatus.WaitScenarioDone)
            {
                scenarioAction.OnMouseMove(mousePosition);
                return;
            }

            if (!CanInput(false))
            {
                return;
            }

            Vector3Int cellPosition = MouseToCellPosition(mousePosition);
            map.mouseCursor.UpdatePosition(cellPosition);
        }

        public override void OnMouseRButtonDown(Vector3 mousePosition)
        {
            if (status == ActionStatus.WaitScenarioDone)
            {
                scenarioAction.OnMouseRButtonDown(mousePosition);
                return;
            }

            if (!CanInput(true))
            {
                return;
            }

            switch (mapStatus)
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
                    map.HideRangeCursors();
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
                if (turn == AttitudeTowards.Player)
                {
                    map.mouseCursor.DisPlayCursor(true);
                }
            }
            else
            {
                map.mouseCursor.DisPlayCursor(false);
            }
        }

        protected virtual void ResetSelected()
        {
            //如果选择了目标
            if (selectedUnit != null)
            {
                //隐藏移动和攻击光标
                map.HideRangeCursors();
                //如果移动过了
                if (movingEndCell != null)
                {
                    //重置格子数据
                    movingEndCell.mapObject = null;
                    selectedCell.mapObject = selectedUnit;
                    movingEndCell = null;
                    //重置角色位置和移动力
                    selectedUnit.UpdatePosition(selectedCell.position);
                    selectedUnit.role.ResetMovePoint();

                    //停止动画
                    selectedUnit.animatorController.StopMove();
                    selectedUnit.animatorController.StopPrepareAttack();
                }

                selectedCell = null;
                selectedUnit = null;
            }

            mapStatus = MapStatus.Normal;
            DisplayMouseCursor(true);
        }
    }
}