using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using Arycs_Fe.ScriptManagement;
using UnityEngine;
using YouYou;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 地图动作类，用来执行地图脚本
    /// </summary>
    public class MapAction : GameAction
    {
        private MapGraph map { get; set; }
        private string nextScene { get; set; } = string.Empty;
        private ActionStatus status { get; set; } = ActionStatus.Error;
        private MapScenarioAction scenarioAction = null;

        private MapStatus mapStatus { get; set; } = MapStatus.Normal;

        private AttitudeTowards turn { get; set; } = AttitudeTowards.Player;
        private int m_TurnToken = 0;

        private CellData selectedCell { get; set; } = null;
        private MapClass selectedUnit { get; set; } = null;
        private MapClass targetUnit { get; set; } = null;
        private CellData movingEndCell { get; set; } = null;

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
            //TODO
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="mapClass"></param>
        /// <param name="moveTo"></param>
        protected void MoveMapClass(MapClass mapClass, CellData moveTo)
        {
            //TODO 
        }

        /// <summary>
        /// 攻击
        /// </summary>
        /// <param name="mapClass"></param>
        /// <param name="target"></param>
        protected void AttackMapClass(MapClass mapClass, MapClass target)
        {
            //TODO
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
            }else
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