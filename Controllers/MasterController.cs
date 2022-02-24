using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;



namespace RegistrationformwithApi.Controllers
{

    [RoutePrefix("api/Student")]
    public class MasterController : ApiController
    {
        string cs = ConfigurationManager.ConnectionStrings["masterdb"].ConnectionString;



        //getall
        [Route("")]
        public IEnumerable<Student> Getall()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("getdata", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                var stdlist = dt.AsEnumerable().Select(row => new Student
                {
                    code = row.Field<int>("code"),
                    firstname = row.Field<string>("firstname"),
                    lastname = row.Field<string>("lastname"),
                    gender = row.Field<int>("gender"),
                    department = row.Field<int>("department"),
                    shtml = row.Field<int>("shtml"),
                    scss = row.Field<int>("scss"),
                    sjavascript = row.Field<int>("sjavascript"),
                    address = row.Field<string>("address"),
                    joiningdate = row.Field<DateTime>("joiningdate"),
                });
                return stdlist;
            }
        }

        //withid
        [Route("{id}")]
        public HttpResponseMessage Getid(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("Proc_getdatawithcode", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@code", id);
                con.Open();
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                var stdlist = dt.AsEnumerable().Select(row => new Student
                {
                    code = row.Field<int>("code"),
                    firstname = row.Field<string>("firstname"),
                    lastname = row.Field<string>("lastname"),
                    gender = row.Field<int>("gender"),
                    department = row.Field<int>("department"),
                    shtml = row.Field<int>("shtml"),
                    scss = row.Field<int>("scss"),
                    sjavascript = row.Field<int>("sjavascript"),
                    address = row.Field<string>("address"),
                    joiningdate = row.Field<DateTime>("joiningdate"),
                });

                var getid = stdlist.FirstOrDefault(e => e.code == id);

                if (getid == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, " id  " + id.ToString() + " not Found");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, stdlist);
                }
            }
        }
        //insert
        [HttpPost]
        [Route("insert")]
        public HttpResponseMessage POST([FromBody] Student student)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand("Proc_insertdata",con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@firstname", student.firstname);
                    cmd.Parameters.AddWithValue("@lastname", student.lastname);
                    cmd.Parameters.AddWithValue("@gender", student.gender);
                    cmd.Parameters.AddWithValue("@department", student.department);
                    cmd.Parameters.AddWithValue("@shtml", student.shtml);
                    cmd.Parameters.AddWithValue("@scss", student.scss);
                    cmd.Parameters.AddWithValue("@sjavascript", student.sjavascript);
                    cmd.Parameters.AddWithValue("@address", student.address);
                    cmd.Parameters.AddWithValue("@joiningdate", student.joiningdate);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    var msg = Request.CreateResponse(HttpStatusCode.Created, student);
                    msg.Headers.Location = new Uri(Request.RequestUri + student.code.ToString());
                    return msg;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        //count

        public int count(int id)
        {
            using(SqlConnection con=new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT count(*)  FROM mstregistation WHERE code=" + id, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                int a = (int)cmd.ExecuteScalar();
                return a;
            }
        }


        //update

        [HttpPut]
        [Route("update/{id}")]
        public HttpResponseMessage Put(int id, [FromBody] Student student)
        {
            try
            {
                int x = count(id);

                if (x>0)
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand("Proc_update", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@firstname", student.firstname);
                        cmd.Parameters.AddWithValue("@lastname", student.lastname);
                        cmd.Parameters.AddWithValue("@gender", student.gender);
                        cmd.Parameters.AddWithValue("@department", student.department);
                        cmd.Parameters.AddWithValue("@shtml", student.shtml);
                        cmd.Parameters.AddWithValue("@scss", student.scss);
                        cmd.Parameters.AddWithValue("@sjavascript", student.sjavascript);
                        cmd.Parameters.AddWithValue("@address", student.address);
                        cmd.Parameters.AddWithValue("@joiningdate", student.joiningdate);
                        cmd.Parameters.AddWithValue("@code",id);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        var msg = Request.CreateResponse(HttpStatusCode.Created, id);
                        return msg;
                    }

                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, " Id =" + id.ToString() + "not fount");
                }
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpDelete]
        [Route("{id}/delete")]
        public HttpResponseMessage Delete(int id, [FromBody] Student student)
        {

            try
            {
                int x = count(id);
                if (x > 0)
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand("Proc_delete", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@code", id);
                        con.Open();
                        cmd.ExecuteNonQuery();

                        return Request.CreateResponse(HttpStatusCode.OK, id);
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, " id =" + id.ToString() + "not fount");

                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }


        }

    }

    public class Student
    {

        public int code { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int gender { get; set; }
        public int department { get; set; }
        public int shtml { get; set; }
        public int scss { get; set; }
        public int   { get; set; }
        public string address { get; set; }
        public DateTime joiningdate { get; set; }
    }
}
