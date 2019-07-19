using ERP_SchoolSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_SchoolSystem.Classes
{
    public static class Get_MonthsAndYears
    {
        static Get_MonthsAndYears()
        {

        }
        public static List<YearList> GetYearList()
        {
            List<YearList> YearList = new List<YearList>();
            int StartYear = 2019;
            int EndYear = DateTime.Now.AddYears(5).Year;
            for (int i = StartYear; i <= EndYear; i++)
            {
                YearList A = new YearList();
                A.Year = i;
                YearList.Add(A);
            }
            return YearList;
        }

        public static List<MonthNameList> GetMonthList()
        {
            List<MonthNameList> MonthList = new List<MonthNameList>();

            MonthNameList A = new MonthNameList();
            A.MonthID = 1;
            A.MonthName = "January";
            MonthList.Add(A);

            MonthNameList B = new MonthNameList();
            B.MonthID = 2;
            B.MonthName = "February";
            MonthList.Add(B);

            MonthNameList C = new MonthNameList();
            C.MonthID = 3;
            C.MonthName = "March";
            MonthList.Add(C);

            MonthNameList D = new MonthNameList();
            D.MonthID = 4;
            D.MonthName = "April";
            MonthList.Add(D);

            MonthNameList E = new MonthNameList();
            E.MonthID = 5;
            E.MonthName = "May";
            MonthList.Add(E);

            MonthNameList F = new MonthNameList();
            F.MonthID = 6;
            F.MonthName = "June";
            MonthList.Add(F);

            MonthNameList G = new MonthNameList();
            G.MonthID = 7;
            G.MonthName = "July";
            MonthList.Add(G);

            MonthNameList H = new MonthNameList();
            H.MonthID = 8;
            H.MonthName = "August";
            MonthList.Add(H);

            MonthNameList I = new MonthNameList();
            I.MonthID = 9;
            I.MonthName = "September";
            MonthList.Add(I);

            MonthNameList J = new MonthNameList();
            J.MonthID = 10;
            J.MonthName = "October";
            MonthList.Add(J);

            MonthNameList K = new MonthNameList();
            K.MonthID = 11;
            K.MonthName = "November";
            MonthList.Add(K);

            MonthNameList L = new MonthNameList();
            L.MonthID = 12;
            L.MonthName = "December";
            MonthList.Add(L);

            return MonthList;
        }
    }
}