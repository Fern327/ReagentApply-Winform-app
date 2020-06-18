using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 入库文件
{
        /// <summary>
        /// 试剂信息
        /// </summary>
        public class Emp
        {
            /// <summary>
            /// 编号
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public String name { get; set; }
            /// <summary>
            /// 生产厂家
            /// </summary>
            public String manufacturer { get; set; }
            /// <summary>
            /// 规模
            /// </summary>
            public String scale { get; set; }
            /// <summary>
            /// 数量
            /// </summary>
            public int count { get; set; }
            /// <summary>
            /// 申请人
            /// </summary>
            public String applyer { get; set; }
            /// <summary>
            /// 申请日期
            /// </summary>
            public String appdate { get; set; }
        }
}
