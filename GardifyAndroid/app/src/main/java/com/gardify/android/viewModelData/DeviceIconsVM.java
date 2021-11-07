package com.gardify.android.viewModelData;

import com.gardify.android.R;

import java.util.ArrayList;
import java.util.List;

public class DeviceIconsVM {

    private int imagePath;
    private String name;

    public static List<DeviceIconsVM> deviceIconsList = new ArrayList<DeviceIconsVM>() {{
       add(new DeviceIconsVM("Gartenbeleuchtung",  R.drawable.icons_geraeteliste_gartenbeleuchtung));
        add(new DeviceIconsVM("Teichpumpe",  R.drawable.icons_geraeteliste_teichpumpe));
        add(new DeviceIconsVM("Regenwassertonne",  R.drawable.icons_geraeteliste_regenwassertonne));
        add(new DeviceIconsVM("Gartenschlauch",  R.drawable.icons_geraeteliste_gartenschlauch));
        add(new DeviceIconsVM("Hochdruckreiniger",  R.drawable.icons_geraeteliste_hochdruckreiniger));
        add(new DeviceIconsVM("Wasseranschluss",  R.drawable.icons_geraeteliste_wasseranschluss));
        add(new DeviceIconsVM("Gewächshaus",  R.drawable.icons_geraeteliste_gewaechshaus));
        add(new DeviceIconsVM("Markise",  R.drawable.icons_geraeteliste_markise));
        add(new DeviceIconsVM("Sonnenschirm",  R.drawable.icons_geraeteliste_sonnenschirm));
        add(new DeviceIconsVM("Sonnensegel",  R.drawable.icons_geraeteliste_sonnensegel));
        add(new DeviceIconsVM("Wasserpumpe",  R.drawable.icons_geraeteliste_wasserpumpe));

    }};

    public DeviceIconsVM(String name, int imagePath) {
        this.imagePath = imagePath;
        this.name = name;
    }

    public int getImagePath() {
        return imagePath;
    }

    public String getName() {
        return name;
    }

}


