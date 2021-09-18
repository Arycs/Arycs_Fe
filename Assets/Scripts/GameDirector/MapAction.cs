using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.Maps;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    public class MapAction : GameAction
    {
        private MapGraph map { get; set; }
        private string nextScene { get; set; } = string.Empty;
        private ActionStatus status { get; set; } = ActionStatus.Error;
        private MapScenarioAction scenarioAction = null;

        private MapStatus m_MapStatus = MapStatus.Normal;

        private AttitudeTowards m_Turn = AttitudeTowards.Player;
        private int m_TurnToken = 0;

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
            //TODO 读取地图脚本，加载资源
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

            return false;
        }
    }
}