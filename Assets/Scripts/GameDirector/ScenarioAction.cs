using System;
using System.Collections;
using System.Collections.Generic;
using Arycs_Fe.ScriptManagement;
using UnityEngine;

namespace Arycs_Fe.ScriptManagement
{
    /// <summary>
    /// 剧本动作类，执行剧本的类
    /// </summary>
    [Serializable]
    public class ScenarioAction : GameAction
    {
        private Iscenario m_Scenario = null;
        private int m_Token = 0;
        private ScenarioActionStatus m_Status = ScenarioActionStatus.Error;

        public Iscenario scenario
        {
            get { return m_Scenario; }
            set { m_Scenario = value; }
        }

        /// <summary>
        /// 执行索引
        /// </summary>
        public int token
        {
            get { return m_Token; }
            set { m_Token = value; }
        }

        /// <summary>
        /// 当前剧本状态
        /// </summary>
        public ScenarioActionStatus status
        {
            get { return m_Status; }
            set { m_Status = value; }
        }

        private readonly SetFlagExecutor m_SetFlagExecutor = new SetFlagExecutor();
        public SetFlagExecutor setFlagExecutor => m_SetFlagExecutor;

        private readonly Dictionary<string, IScenarioContentExecutor> m_ExecutorDict =
            new Dictionary<string, IScenarioContentExecutor>();

        private readonly Dictionary<string, int> m_FlagDict = new Dictionary<string, int>();

        /// <summary>
        /// 添加或设置Executor
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="existOverride"></param>
        /// <returns></returns>
        public bool SetExecutor(IScenarioContentExecutor executor, bool existOverride = true)
        {
            if (string.IsNullOrEmpty(executor?.code))
            {
                return false;
            }

            if (!existOverride && m_ExecutorDict.ContainsKey(executor.code))
            {
                return false;
            }

            m_ExecutorDict[executor.code] = executor;
            return true;
        }

        /// <summary>
        /// 获取Executor
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IScenarioContentExecutor GetExecutor(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return null;
            }

            return !m_ExecutorDict.TryGetValue(code, out IScenarioContentExecutor executor) ? null : executor;
        }

        /// <summary>
        /// 初始化Executor
        /// </summary>
        /// <param name="executorTypes"></param>
        public void LoadExecutors(params Type[] executorTypes)
        {
            if (executorTypes == null)
            {
                return;
            }

            for (int i = 0; i < executorTypes.Length; i++)
            {
                IScenarioContentExecutor executor =
                    Activator.CreateInstance(executorTypes[i]) as IScenarioContentExecutor;
                SetExecutor(executor);
            }
        }

        /// <summary>
        /// 读取剧本
        /// </summary>
        /// <param name="scenario"></param>
        /// <returns></returns>
        public bool LoadScenario(Iscenario scenario)
        {
            if (scenario == null || !scenario.isLoaded)
            {
                //error = $"{GetType().Name} -> LoadScenario: 'scenario' is null or 'scenario' is not loaded";
                error = $"{GetType().Name} -> 读取剧本: 剧本是空, 或者已经读取过剧本";
                return false;
            }

            this.scenario = scenario;
            this.status = ScenarioActionStatus.Continue;
            this.token = 0;
            this.m_FlagDict.Clear();
            return true;
        }

        /// <summary>
        /// 执行每一步命令
        /// </summary>
        /// <returns></returns>
        private ScenarioActionStatus Step()
        {
            if (token >= scenario.contentCount)
            {
                error = $"{GetType().Name} -> 执行步骤 : 剧本执行完毕";
                return ScenarioActionStatus.Error;
            }

            IScenarioContent content = scenario.GetContent(token);
            Debug.LogErrorFormat("执行步骤 {0}:{1}", token, content.ToString());

            IScenarioContentExecutor executor;
            //如果是标识符，设置标识符
            if (content.type == ScenarioContentType.Flag)
            {
                executor = setFlagExecutor;
            }
            //如果是动作，执行动作
            else
            {
                executor = GetExecutor(content.code);
                if (executor == null)
                {
                    //error = $"{GetType().Name} -> Step : executor '{content.code}' was not fount";
                    error = $"{GetType().Name} -> 执行步骤 : 解析器 '{content.code}' 未被找到";
                    return ScenarioActionStatus.Error;
                }
            }

            token++;
            ScenarioActionStatus result = executor.Execute(this, content, out m_Error);

            return result;
        }

        public override bool Update()
        {
            while (status == ScenarioActionStatus.Continue)
            {
                status = Step();
            }

            //如果出错了，就中断
            if (status == ScenarioActionStatus.Error)
            {
                Abort();
                return false;
            }
            //等待下一帧
            else if (status == ScenarioActionStatus.NextFrame)
            {
                status = ScenarioActionStatus.Continue;
            }

            return true;
        }

        /// <summary>
        /// 检查并设置剧情标识符
        /// </summary>
        /// <param name="flag">标识符</param>
        /// <param name="cmdError">错误日志</param>
        /// <returns></returns>
        public ScenarioActionStatus SetFlagCommand(string flag, out string cmdError)
        {
            if (m_FlagDict.TryGetValue(flag, out int index))
            {
                //如果已经存在的标识符重名，并且值不等，那么说明标识符不唯一
                if (index != token)
                {
                    // cmdError = $"{GetType().Name} -> SetFlagCommand:flag '{flag}' is already exist";
                    cmdError = $"{GetType().Name} -> 设置剧情标识符:标识符 '{flag}' 已经存在";
                    return ScenarioActionStatus.Error;
                }
            }
            else
            {
                m_FlagDict.Add(flag, token);
            }

            cmdError = null;
            return ScenarioActionStatus.Continue;
        }

        /// <summary>
        /// 将剧本跳转到Flag
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="cmdError"></param>
        /// <returns></returns>
        public ScenarioActionStatus GotoCommand(string flag, out string cmdError)
        {
            if (!m_FlagDict.TryGetValue(flag, out int index))
            {
                //向后查找flag
                while (token < scenario.contentCount)
                {
                    IScenarioContent content = scenario.GetContent(token);
                    token++;
                    if (content.type != ScenarioContentType.Flag)
                    {
                        continue;
                    }

                    //向后查找时,将新的剧情标识符加入到字典中
                    if (setFlagExecutor.Execute(this, content, out cmdError) == ScenarioActionStatus.Error)
                    {
                        return ScenarioActionStatus.Error;
                    }

                    // 是我们需要的剧情标识符
                    if (flag == content.code)
                    {
                        return ScenarioActionStatus.Continue;
                    }
                }

                //没有搜索到
                //cmdError = string.Format("{0} GotoCommand error:flag '{1}' was not found", GetType().Name, flag);
                cmdError = $"{GetType().Name} 跳转指令 错误:标识符 '{flag}' 未被找到";
                return ScenarioActionStatus.Error;
            }

            token = index;
            cmdError = null;
            return ScenarioActionStatus.Continue;
        }

        public override void Dispose()
        {
            base.Dispose();
            m_Scenario = null;
            m_Status = ScenarioActionStatus.Error;
            m_Token = 0;
            m_ExecutorDict.Clear();
            m_FlagDict.Clear();
        }

        public ScenarioAction() : base()
        {
        }

        public ScenarioAction(IGameAction previous) : base(previous)
        {
        }
    }
}