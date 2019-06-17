using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.Models;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace ChainStore.Data.Traditional
{
    public class Qdatabase
    {
        private SqlConnection connection;
        
        public Qdatabase()
        {
            connection = new SqlConnection("data source = DESKTOP-GO2FS9V; initial catalog = ChainStore; integrated security = true");
        }

        public List<Appointments> retAppointment()
        {
            List<Appointments> ls = new List<Appointments>();
            SqlCommand cmd = new SqlCommand("appointment",connection);
            cmd.CommandType = CommandType.StoredProcedure;
            connection.Open();
            var read = cmd.ExecuteReader();
            while (read.Read())
            {
                ls.Add(new Appointments()
                {
                    Id = Convert.ToInt32(read[0]),
                    AppointmentDate = Convert.ToDateTime(read[1]),
                    CustomerName = read[2].ToString(),
                    CustomerNumber = read[3].ToString(),
                    CustomerEmail = read[4].ToString(),
                    IsConfirmed = Convert.ToInt32(read[5]) == 1 ? true : false
                });
            }
            connection.Close();
            
            return ls;
        }

        public Appointments retAppointment(int id)
        {
            Appointments p = null;
            SqlCommand cmd = new SqlCommand("retappointmentf", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt = new SqlParameter("@id", SqlDbType.Int);
            pmt.Value = id;
            cmd.Parameters.Add(pmt);
            connection.Open();
            var read = cmd.ExecuteReader();
            while (read.Read())
            {
                p = new Appointments()
                {
                    Id = Convert.ToInt32(read[0]),
                    AppointmentDate = Convert.ToDateTime(read[1]),
                    CustomerName = read[2].ToString(),
                    CustomerNumber = read[3].ToString(),
                    CustomerEmail = read[4].ToString(),
                    IsConfirmed = Convert.ToInt32(read[5]) == 1 ? true : false
                };
            }
            connection.Close();

            return p;
        }

        public void upAppointment(Appointments input)
        {
            SqlCommand cmd = new SqlCommand("upappointment", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt1 = new SqlParameter("@Id", SqlDbType.Int);
            pmt1.Value = input.Id;
            SqlParameter pmt2 = new SqlParameter("@AppointmentDate", SqlDbType.DateTime);
            pmt2.Value = Convert.ToDateTime(input.AppointmentDate);
            SqlParameter pmt3 = new SqlParameter("@CustomerName", SqlDbType.NVarChar);
            pmt3.Value = input.CustomerName;
            SqlParameter pmt4 = new SqlParameter("@CustomerNumber", SqlDbType.NVarChar);
            pmt4.Value = input.CustomerNumber;
            SqlParameter pmt5 = new SqlParameter("@CustomerEmail", SqlDbType.NVarChar);
            pmt5.Value = input.CustomerEmail;
            SqlParameter pmt6 = new SqlParameter("@IsConfirmed", SqlDbType.Bit);
            pmt6.Value = input.IsConfirmed ? 1 : 0;
            cmd.Parameters.Add(pmt1); cmd.Parameters.Add(pmt2); cmd.Parameters.Add(pmt3); cmd.Parameters.Add(pmt4); cmd.Parameters.Add(pmt5); cmd.Parameters.Add(pmt6);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void rmappointment(int id)
        {
            SqlCommand cmd = new SqlCommand("rmappointment", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt = new SqlParameter("@id", SqlDbType.Int);
            pmt.Value = id;
            cmd.Parameters.Add(pmt);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public List<Appointments> SucAppointments()
        {
            List<Appointments> ls = new List<Appointments>();
            SqlCommand cmd = new SqlCommand("sucappointment", connection);
            connection.Open();
            var read = cmd.ExecuteReader();
            while (read.Read())
            {
                ls.Add(new Appointments()
                {
                    Id = Convert.ToInt32(read[0]),
                    AppointmentDate = Convert.ToDateTime(read[1]),
                    CustomerName = read[2].ToString(),
                    CustomerNumber = read[3].ToString(),
                    CustomerEmail = read[4].ToString(),
                    IsConfirmed = Convert.ToInt32(read[5]) == 1 ? true : false
                });
            }
            connection.Close();

            return ls;

        }

        public List<ProductsSelectedForAppointment> retpsa(int id)
        {
            SqlCommand cmd = new SqlCommand("retpsa", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt = new SqlParameter("@id", SqlDbType.Int);
            pmt.Value = id;
            cmd.Parameters.Add(pmt);
            List<ProductsSelectedForAppointment> ls = new List<ProductsSelectedForAppointment>();
            connection.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ls.Add(new ProductsSelectedForAppointment()
                {
                    Id = Convert.ToInt32(reader[0]),
                    AppointmentId = Convert.ToInt32(reader[1]),
                    ProductId = Convert.ToInt32(reader[2]),
                    Count = Convert.ToInt32(reader[3])
                });
            }

            connection.Close();
            return ls;
        }

        public void include_pt_st(List<Products> ls)
        {
            foreach (Products i in ls)
            {
                i.SpecialTags = retSpecialTag(i.SpecialTagId);
                i.ProductTypes = retProductType(i.ProductTypeId);
            }
        }

        public List<Products> p_join_psa(int id)
        {
            SqlCommand cmd = new SqlCommand("p_psd_join", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt = new SqlParameter("@id", SqlDbType.Int);
            pmt.Value = id;
            cmd.Parameters.Add(pmt);
            connection.Open();
            var reader = cmd.ExecuteReader();
            List<Products> ls = new List<Products>();
            while (reader.Read())
            {
                ls.Add(new Products()
                {
                    Id = Convert.ToInt32(reader[0]),
                    Name = reader[1].ToString(),
                    Image = reader[2].ToString(),
                    Price = Convert.ToInt32(reader[3]),
                    Available = Convert.ToInt32(reader[4]) == 1 ? true : false,
                    ProductTypeId = Convert.ToInt32(reader[5]),
                    SpecialTagId = Convert.ToInt32(reader[6]),
                    Count = Convert.ToInt32(reader[7])
                });
            }
            connection.Close();
            

            return ls;
        }

        public List<Products> retProduct()
        {
            List<Products> ls = new List<Products>();
            SqlCommand cmd = new SqlCommand("retproduct", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            connection.Open();
            var read = cmd.ExecuteReader();
            while (read.Read())
            {
                ls.Add(new Products()
                {
                    Id = Convert.ToInt32(read[0]),
                    Name = read[1].ToString(),
                    Image = read[2].ToString(),
                    Price = Convert.ToDouble(read[3]),
                    Available = Convert.ToInt32(read[4]) == 1 ? true : false,
                    ProductTypeId = Convert.ToInt32(read[5]),
                    SpecialTagId = Convert.ToInt32(read[6]),
                    Count = Convert.ToInt32(read[7])
                });
            }
            connection.Close();

            return ls;
        }

        public Products retProduct(int id)
        {
            SqlCommand cmd = new SqlCommand("retproduct", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt = new SqlParameter("@id", SqlDbType.Int);
            pmt.Value = id;
            cmd.Parameters.Add(pmt);
            connection.Open();
            var read = cmd.ExecuteReader();
            Products p = null;
            while (read.Read())
            {
                p = new Products()
                {
                    Id = Convert.ToInt32(read[0]),
                    Name = read[1].ToString(),
                    Image = read[2].ToString(),
                    Price = Convert.ToDouble(read[3]),
                    Available = Convert.ToInt32(read[4]) == 1 ? true : false,
                    ProductTypeId = Convert.ToInt32(read[5]),
                    SpecialTagId = Convert.ToInt32(read[6]),
                    Count = Convert.ToInt32(read[7])
                };
            }
            connection.Close();

            return p;
        }

        public Products retProduct(Products input)
        {
            SqlCommand cmd = new SqlCommand("retproductwithoutid", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt1 = new SqlParameter("@name", SqlDbType.NVarChar);
            pmt1.Value = input.Name;
            SqlParameter pmt2 = new SqlParameter("@image", SqlDbType.NVarChar);
            pmt2.Value = input.Image == null ? "default" : input.Image;
            SqlParameter pmt3 = new SqlParameter("@price", SqlDbType.Float);
            pmt3.Value = input.Price;
            SqlParameter pmt4 = new SqlParameter("@available", SqlDbType.Bit);
            pmt4.Value = input.Available ? 1 : 0;
            SqlParameter pmt5 = new SqlParameter("@ptid", SqlDbType.Int);//ptid = producttype id
            pmt5.Value = input.ProductTypeId;
            SqlParameter pmt6 = new SqlParameter("@stid", SqlDbType.Int);//stid = specialtag id
            pmt6.Value = input.SpecialTagId;
            SqlParameter pmt7 = new SqlParameter("@count", SqlDbType.Int);
            pmt7.Value = input.Count;
            cmd.Parameters.Add(pmt1); cmd.Parameters.Add(pmt2); cmd.Parameters.Add(pmt3); cmd.Parameters.Add(pmt4); cmd.Parameters.Add(pmt5); cmd.Parameters.Add(pmt6); cmd.Parameters.Add(pmt7);
            connection.Open();
            var read = cmd.ExecuteReader();
            Products p = null;
            while (read.Read())
            {
                p = new Products()
                {
                    Id = Convert.ToInt32(read[0]),
                    Name = read[1].ToString(),
                    Image = read[2].ToString(),
                    Price = Convert.ToDouble(read[3]),
                    Available = Convert.ToInt32(read[4]) == 1 ? true : false,
                    ProductTypeId = Convert.ToInt32(read[5]),
                    SpecialTagId = Convert.ToInt32(read[6]),
                    Count = Convert.ToInt32(read[7])
                };
            }
            connection.Close();

            return p;
        }


        public void upProduct(Products input)
        {
            SqlCommand cmd = new SqlCommand("upproduct", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt0 = new SqlParameter("@id", SqlDbType.NVarChar);
            pmt0.Value = input.Id;
            SqlParameter pmt1 = new SqlParameter("@name", SqlDbType.NVarChar);
            pmt1.Value = input.Name;
            SqlParameter pmt2 = new SqlParameter("@image", SqlDbType.NVarChar);
            pmt2.Value = input.Image == null ? "default" : input.Image;
            SqlParameter pmt3 = new SqlParameter("@price", SqlDbType.Float);
            pmt3.Value = input.Price;
            SqlParameter pmt4 = new SqlParameter("@available", SqlDbType.Bit);
            pmt4.Value = input.Available ? 1 : 0;
            SqlParameter pmt5 = new SqlParameter("@ptid", SqlDbType.Int);//ptid = producttype id
            pmt5.Value = input.ProductTypeId;
            SqlParameter pmt6 = new SqlParameter("@stid", SqlDbType.Int);//stid = specialtag id
            pmt6.Value = input.SpecialTagId;
            SqlParameter pmt7 = new SqlParameter("@count", SqlDbType.Int);
            pmt7.Value = input.Count;
            cmd.Parameters.Add(pmt0); cmd.Parameters.Add(pmt1); cmd.Parameters.Add(pmt2); cmd.Parameters.Add(pmt3); cmd.Parameters.Add(pmt4); cmd.Parameters.Add(pmt5); cmd.Parameters.Add(pmt6); cmd.Parameters.Add(pmt7);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();

        }

        public void inProduct(Products input)
        {
            SqlCommand cmd = new SqlCommand("inproduct", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt1 = new SqlParameter("@name",SqlDbType.NVarChar);
            pmt1.Value = input.Name;
            SqlParameter pmt2 = new SqlParameter("@image", SqlDbType.NVarChar);
            pmt2.Value = input.Image == null ? "default" : input.Image;
            SqlParameter pmt3 = new SqlParameter("@price", SqlDbType.Float);
            pmt3.Value = input.Price;
            SqlParameter pmt4 = new SqlParameter("@available", SqlDbType.Bit);
            pmt4.Value = input.Available ? 1 : 0;
            SqlParameter pmt5 = new SqlParameter("@ptid", SqlDbType.Int);//ptid = producttype id
            pmt5.Value = input.ProductTypeId;
            SqlParameter pmt6 = new SqlParameter("@stid", SqlDbType.Int);//stid = specialtag id
            pmt6.Value = input.SpecialTagId;
            SqlParameter pmt7 = new SqlParameter("@count", SqlDbType.Int);
            pmt7.Value = input.Count;
            cmd.Parameters.Add(pmt1); cmd.Parameters.Add(pmt2); cmd.Parameters.Add(pmt3); cmd.Parameters.Add(pmt4); cmd.Parameters.Add(pmt5); cmd.Parameters.Add(pmt6); cmd.Parameters.Add(pmt7);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }



        public List<Special_Tags> retSpecialTag()
        {
            List<Special_Tags> ls = new List<Special_Tags>();
            SqlCommand cmd = new SqlCommand("specialtag", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            connection.Open();
            var read = cmd.ExecuteReader();
            while (read.Read())
            {
                ls.Add(new Special_Tags()
                {
                    Id = Convert.ToInt32(read[0]),
                    Name = read[1].ToString()
                });
            }
            connection.Close();

            return ls;
        }

        public Special_Tags retSpecialTag(int id)
        {
            SqlCommand cmd = new SqlCommand("fspecialtag", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt = new SqlParameter("@id", SqlDbType.Int);
            pmt.Value = id;
            cmd.Parameters.Add(pmt);
            connection.Open();
            var read = cmd.ExecuteReader();
            Special_Tags tag = null;
            while (read.Read())
            {
                tag = new Special_Tags()
                {
                    Id = Convert.ToInt32(read[0]),
                    Name = read[1].ToString()
                };
            }
            connection.Close();

            return tag;

        }

        public void InSpecialTagses(Special_Tags input)
        {
            //List<Special_Tags> ls = new List<Special_Tags>();
            SqlCommand cmd = new SqlCommand("inspacialtag", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt = new SqlParameter("@name", System.Data.SqlDbType.NVarChar);
            pmt.Value = input.Name;
            cmd.Parameters.Add(pmt);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void upSpecialTag(Special_Tags input)
        {
            SqlCommand cmd = new SqlCommand("upspecialtag", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt = new SqlParameter("@name", System.Data.SqlDbType.NVarChar);
            SqlParameter pmt2 = new SqlParameter("@id", SqlDbType.Int);
            pmt.Value = input.Name;
            pmt2.Value = input.Id;
            cmd.Parameters.Add(pmt);
            cmd.Parameters.Add(pmt2);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void rmSpecialTag(int id)
        {
            SqlCommand cmd = new SqlCommand("rmspecialtag", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt = new SqlParameter("@id", System.Data.SqlDbType.Int);
            pmt.Value = id;
            cmd.Parameters.Add(pmt);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }


        public List<ProductTypes> retProductType()
        {
            List<ProductTypes> ls = new List<ProductTypes>();
            SqlCommand cmd = new SqlCommand("producttype", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            connection.Open();
            var read = cmd.ExecuteReader();
            while (read.Read())
            {
                ls.Add(new ProductTypes()
                {
                    Id = Convert.ToInt32(read[0]),
                    Name = read[1].ToString()
                });
            }
            connection.Close();

            return ls;
        }

        public ProductTypes retProductType(int id)
        {
            SqlCommand cmd = new SqlCommand("fproducttype", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt = new SqlParameter("@id", SqlDbType.Int);
            pmt.Value = id;
            cmd.Parameters.Add(pmt);
            connection.Open();
            var read = cmd.ExecuteReader();
            ProductTypes pt = null;
            while (read.Read())
            {
                pt = new ProductTypes()
                {
                    Id = Convert.ToInt32(read[0]),
                    Name = read[1].ToString()
                };
            }
            connection.Close();

            return pt;

        }

        public void rmproduct(int id)
        {
            SqlCommand cmd = new SqlCommand("rmproduct", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt = new SqlParameter("@id", System.Data.SqlDbType.Int);
            pmt.Value = id;
            cmd.Parameters.Add(pmt);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void Inproducttype(ProductTypes input)
        {
            //List<Special_Tags> ls = new List<Special_Tags>();
            SqlCommand cmd = new SqlCommand("inproducttype", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt = new SqlParameter("@name", System.Data.SqlDbType.NVarChar);
            pmt.Value = input.Name;
            cmd.Parameters.Add(pmt);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }



        public void upproducttype(ProductTypes input)
        {
            SqlCommand cmd = new SqlCommand("upproducttypes", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt = new SqlParameter("@name", System.Data.SqlDbType.NVarChar);
            SqlParameter pmt2 = new SqlParameter("@id", SqlDbType.Int);
            pmt.Value = input.Name;
            pmt2.Value = input.Id;
            cmd.Parameters.Add(pmt);
            cmd.Parameters.Add(pmt2);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void rmproducttype(int id)
        {
            SqlCommand cmd = new SqlCommand("rmproducttype", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pmt = new SqlParameter("@id", System.Data.SqlDbType.Int);
            pmt.Value = id;
            cmd.Parameters.Add(pmt);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }


    }
}
