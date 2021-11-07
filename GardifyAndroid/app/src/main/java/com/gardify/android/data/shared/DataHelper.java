package com.gardify.android.data.shared;

import android.graphics.Color;

import java.util.HashMap;
import java.util.Map;

import static android.graphics.Color.rgb;

public class DataHelper {

   public static Map<String, Integer> colorsMap = new HashMap<String, Integer>() {
        {
            put("rot", Color.rgb(255, 0, 0));
            put("weiß", Color.rgb(255, 255, 255));
            put("beige", Color.rgb(233, 215, 187));
            put("gelb", Color.rgb(255,255,0));
            put("gelblich", Color.rgb(255,255,153));
            put("hellorange", Color.rgb(255, 206 ,85));
            put("orange", Color.rgb(255, 92, 32));
            put("blau", Color.rgb(0, 81 , 255));
            put("petrol", Color.rgb(20, 96, 126));
            put("grau", Color.rgb(172, 172, 172));
            put("grün", 	Color.rgb(0,128,0));
            put("schwarz", Color.rgb(0,0,0));
            put("braun", 	Color.rgb(165,42,42));
            put("pink", Color.rgb(255, 111, 199));
            put("apricot", Color.rgb(245, 149, 86));
            put("creme", Color.rgb(231, 225, 204));
            put("violett", Color.rgb(189, 125, 241));
            put("rosa", Color.rgb(250, 57, 176));
            put("purpur", Color.rgb(226, 21, 245));
            put("hellblau", Color.rgb(83, 169, 226));
            put("dunkelrot", Color.rgb(175, 0, 0));
            put("blaugrün", Color.rgb(33, 201, 187));
            put("gelbgrün", Color.rgb(166, 190, 25));
            put("rotbraun", Color.rgb(119, 18, 18));
            put("lachsfarben", Color.rgb(236, 179, 179));
            put("blauviolett", Color.rgb(55, 78, 172));
            put("silbrigweiß", Color.rgb(204, 204, 204));
            put("fliederfarben", Color.rgb(192, 147, 199));
        }
    };
}
