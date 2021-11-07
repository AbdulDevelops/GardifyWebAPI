package com.gardify.android.viewModelData;

import java.util.ArrayList;
import java.util.List;

public class BadgesIconVM {

    private String name;
    private String image;
    private String id;
    private boolean checked;

    public static List<BadgesIconVM> EcoBadges = new ArrayList<BadgesIconVM>() {{
        add(new BadgesIconVM("447", "Bienenfreundlich", "bienenfreundlich", false));
        add(new BadgesIconVM("321", "Insektenfreundlich", "insektenfreundlich", false));
        add(new BadgesIconVM("530", "Heimische Pflanze", "heimische_pflanzen", false));
        add(new BadgesIconVM("445", "Ökologisch wertvoll", "oekologisch_wertvoll", false));
        add(new BadgesIconVM("320,322", "Vogelfreundlich", "vogelfreundlich", false));
        add(new BadgesIconVM("531", "Schmetterlingsfreundlich", "schmetterlingsfreundlich", false));
        add(new BadgesIconVM("346", "Wassersparende Pflanzen", "wassersparende_pflanzen", false));
    }};

    public static List<BadgesIconVM> FrostBadges = new ArrayList<BadgesIconVM>() {{
        add(new BadgesIconVM("295", "nicht frosthart", "nicht_frosthart", false));
        add(new BadgesIconVM("293", "frosthart bis -5 °C", "frosthart_bis_5", false));
        add(new BadgesIconVM("292", "frosthart bis -10 °C", "frosthart_bis_10", false));
        add(new BadgesIconVM("285", "voll frosthart", "voll_frosthart", false));
        add(new BadgesIconVM("315", "bedingt giftig", "bedingt_giftig", false));
        add(new BadgesIconVM("561", "stark giftig", "stark_giftig", false));
    }};

    public BadgesIconVM(String id, String name, String image, boolean checked) {
        this.name = name;
        this.id = id;
        this.image =image;
        this.checked = checked;
    }

    public String getImage() {
        return image;
    }

    public void setImage(String image) {
        this.image = image;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public boolean isChecked() {
        return checked;
    }

    public void setChecked(boolean checked) {
        this.checked = checked;
    }

}


