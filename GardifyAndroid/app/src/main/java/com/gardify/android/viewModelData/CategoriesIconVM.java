package com.gardify.android.viewModelData;

import com.gardify.android.R;

import java.util.ArrayList;
import java.util.List;

public class CategoriesIconVM {

    private int imagePath;
    private String name;

    public static List<CategoriesIconVM> filterCategories = new ArrayList<CategoriesIconVM>() {{
       add(new CategoriesIconVM("Ausschlusskriterien",  R.drawable.gardify_app_icon_ausschlusskriterien));
        add(new CategoriesIconVM("Besonderheiten",  R.drawable.gardify_app_icon_besonderheiten));
        add(new CategoriesIconVM("Herbstfärbung",  R.drawable.gardify_app_icon_herbstfaerbung));
        add(new CategoriesIconVM("Wuchs",  R.drawable.gardify_app_icon_wuchs));
        add(new CategoriesIconVM("Nutzpflanzen",  R.drawable.gardify_app_icon_nutzpflanzen));
        add(new CategoriesIconVM("Dekoaspekte",  R.drawable.gardify_app_icon_dekoaspekte));

        add(new CategoriesIconVM("Blüten",  R.drawable.gardify_app_icon_blueten));
        add(new CategoriesIconVM("Blütenform",  R.drawable.gardify_app_icon_bluetenform));
        add(new CategoriesIconVM("Blütengröße",  R.drawable.gardify_app_icon_bluetengroesse));
        add(new CategoriesIconVM("Fruchtfarbe",  R.drawable.gardify_app_icon_fruchtfarbe));
        add(new CategoriesIconVM("Früchte",  R.drawable.gardify_app_icon_fruechte));
        add(new CategoriesIconVM("Blütenstand",  R.drawable.gardify_app_icon_bluetenstand));

        add(new CategoriesIconVM("Blattfarbe",  R.drawable.gardify_app_icon_blattfarbe));
        add(new CategoriesIconVM("Blattrand",  R.drawable.gardify_app_icon_blattrand));
        add(new CategoriesIconVM("Blattstellung",  R.drawable.gardify_app_icon_blattstellung));
        add(new CategoriesIconVM("Blattform",  R.drawable.gardify_app_icon_blattform));
        add(new CategoriesIconVM("Laubrhythmus", R.drawable.gardify_app_icon_laubrhythmus));

        add(new CategoriesIconVM("Boden",  R.drawable.gardify_app_icon_boden));
        add(new CategoriesIconVM("Licht",  R.drawable.gardify_app_icon_licht));
        add(new CategoriesIconVM("Düngung",  R.drawable.gardify_app_icon_duengung));
        add(new CategoriesIconVM("Schnitt",  R.drawable.gardify_app_icon_schnitt));
        add(new CategoriesIconVM("Vermehrung",  R.drawable.gardify_app_icon_vermehrung));
        add(new CategoriesIconVM("Wasserbedarf",  R.drawable.gardify_app_icon_wasserbedarf));

    }};

    public CategoriesIconVM(String name, int imagePath) {
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


