using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class StatisticEvent
    {
        public int StatisticEventId { get; set; }
        public string Name { get; set; }
    }

    public enum StatisticEventTypes
    {
        Login = 1, 
        Register = 2, 
        SuggestPlant = 3, 
        OrderConfirmed = 4,
        OrderClicked = 5, // not yet tracked
        Pageview = 6,
        NewGarden = 7, 
        EmailNotify = 8, // not yet tracked
        SubmitQuestion = 9, 
        SubmitAnswer = 10,
        AddToCart = 11,
        AddGardenPlant = 12,
        SaveBioScan = 13,
        GuidedTour = 14,
        TodoEntry = 15,
        DiaryEntry=16,
        CyclicTodoEntry=17,
        AdClicked = 18,
        RegisterDemo=19,
        RegisterConverted=20,
        LoginiOS = 21,
        LoginAndroid = 22,
        RegisteriOS = 23,
        RegisterAndroid = 24,
        RegisterWebPage=25,
        LoginWebPage=26,
        ApiCallFromIos=27,
        ApiCallFromAndroid=28,
        ApiCallFromWebpage=29,
       
    }
}