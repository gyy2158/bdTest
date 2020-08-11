using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RabbitSqlLib;
using TaskFront.model;

namespace TaskFront.Controllers
{
    [Microsoft.AspNetCore.Cors.EnableCors("AllowAllOrigins")]
    [Route("api/[controller]")]

    [ApiController]
    public class XmController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        public class model2
        {
            public string[] str { get; set; }
        }

        [HttpPost("GetTsJHInfo")]
        public StateInfo GetTsJHInfo([FromForm] List<string> str)
        {
            //string[] s2 = str.Split(',');
            string s = "'" + string.Join("','", str.ToArray()) + "'";
            StateInfo si = new StateInfo();
            RabbitAccess access = DBConnection();
            DataTable dt = access.GetDataTable("select * from ts_j_basicinfo where BZJH in (" + s + ")");
            si.data = dt;
            return si;
        }

        public RabbitAccess DBConnection()
        {
            RabbitSqlLib.DBEntity dBEntity = new DBEntity();
            dBEntity.DBType = "oracle";
            dBEntity.DBPort = "1521";
            dBEntity.DBServer = "132.232.16.136";
            dBEntity.DBName = "orcl";
            dBEntity.DBUser = "dqts";
            dBEntity.DBPwd = "dqts";
            string dBType = "";
            string connstr = dBEntity.GetConnStr(out dBType);
            RabbitAccess access = new RabbitAccess(dBType, connstr);
            return access;
        }

        [HttpGet("GetInfo")]
        public StateInfo GetInfo([FromQuery] string str)
        {
            string[] s2 = str.Split(',');
            string s = string.Join(",", s2.ToArray());
            StateInfo si = new StateInfo();
            RabbitAccess access = DBConnection();
            DataTable dt = access.GetDataTable("select * from ts_j_basicinfo where BZJH in (" + s + ")");
            si.data = dt;
            return si;
        }

        [HttpPost("GetTsJH")]
        public StateInfo GetTsJH([FromForm] PageHelper ph)
        {
            StateInfo si = new StateInfo();
            int begin_num = (ph.page - 1) * ph.limit;
            int end_num = ph.page * ph.limit;
            RabbitAccess access = DBConnection();
            string sql = "select * from ( " +
            "select row_limit.*, rownum rownum_ from(" +
            "select count_num.*, count(1)over() totalnum_ from(" +
            "select * from ts_j_basicinfo where jh like '%"+ph.jhmc+"%' order by bzjh, tcrq" +
            ") count_num" +
            ") row_limit where rownum <= " + end_num + "" +
            ")where rownum_ > " + begin_num + "";
            DataTable dt = access.GetDataTable(sql);
            si.data = dt;
            return si;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
