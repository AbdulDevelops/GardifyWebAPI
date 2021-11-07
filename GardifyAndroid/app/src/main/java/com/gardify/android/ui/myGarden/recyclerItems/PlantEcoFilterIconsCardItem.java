package com.gardify.android.ui.myGarden.recyclerItems;

import android.content.Context;
import android.graphics.Color;
import android.os.Bundle;
import android.util.Log;
import android.view.View;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.data.myGarden.Badge;
import com.gardify.android.data.myGarden.MyGarden;
import com.gardify.android.databinding.RecyclerViewMyGardenPlantFilterIconsItemBinding;
import com.gardify.android.viewModelData.BadgesIconVM;
import com.xwray.groupie.databinding.BindableItem;

import java.util.ArrayList;
import java.util.List;

public class PlantEcoFilterIconsCardItem extends BindableItem<RecyclerViewMyGardenPlantFilterIconsItemBinding> {

    public static final String BEE_FRIENDLY_KEY = "447";
    public static final String BIRD_FRIENDLY_KEY_1 = "320";
    public static final String BIRD_FRIENDLY_KEY_2 = "322";
    public static final String INSECT_FRIENDLY_KEY = "321";
    public static final String ECOLOGICAL_PLANT_KEY = "445";
    public static final String WATER_SAVING_PLANT_KEY = "346";
    public static final String BUTTERFLY_FRIENDLY_KEY = "531";
    public static final String NATIVE_PLANT_KEY = "530";
    public static final String NOT_FROST_RESISTANT = "295";
    public static final String FROST_MINUS_FIVE = "293";
    public static final String FROST_MINUS_TEN = "292";
    public static final String COMPLETELY_FROST_RESISTANT = "285";
    public static final String CONDITIONALLY_POISONOUS = "315";
    public static final String HIGHLY_POISONOUS = "561";
    private CharSequence text;
    private Context context;
    private List<MyGarden> myGardenList;
    private int bgColor;
    private int ecoBadgeNumber, frostBadgeNumber;
    private long beeFriendlyCount, birdFriendlyCount, insectFriendlyCount, nativeCount, ecologicalCount, butterflyFriendlyCount,
            waterSavingCount;
    private String selectedEcoTag = "", selectedFrostTag = "";
    public static List<String> appliedFilterList = new ArrayList<String>();

    public PlantEcoFilterIconsCardItem(Context context, int bgColor, List<MyGarden> myGardenList, CharSequence text) {
        this.context = context;
        this.bgColor = bgColor;
        this.myGardenList = myGardenList;
        this.text = text;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_my_garden_plant_filter_icons_item;
    }

    @Override
    public void bind(@NonNull RecyclerViewMyGardenPlantFilterIconsItemBinding _binding, int position) {

        //setting counter
        _binding.textViewMyGardenAppliedFilter.setVisibility(View.GONE);
        _binding.linearLayoutOuterLl.setBackgroundColor(context.getResources().getColor(bgColor, null));
        _binding.linearLayout.setBackgroundColor(context.getResources().getColor(bgColor, null));
        _binding.textViewMyGardenBienenCount.setText(String.valueOf(beeFriendlyCount));
        _binding.textViewMyGardenVogelfreundlichCount.setText(String.valueOf(birdFriendlyCount));
        _binding.textViewMyGardenInsektenfreundlichCount.setText(String.valueOf(insectFriendlyCount));
        _binding.textViewMyGardenOekologischCount.setText(String.valueOf(ecologicalCount));
        _binding.textViewMyGardenSchmetterlingsfreundlichCount.setText(String.valueOf(butterflyFriendlyCount));
        _binding.textViewMyGardenHeimischeCount.setText(String.valueOf(nativeCount));
        _binding.textViewMyGardenWassersparendePflanzenCount.setText(String.valueOf(waterSavingCount));
        Log.d("selEcoTag", "bind: " + selectedEcoTag + " frostTag: " + selectedFrostTag);
        if (!selectedEcoTag.isEmpty()) {
            if (Integer.parseInt(selectedEcoTag) >= 285 && Integer.parseInt(selectedEcoTag) <= 315
                    || selectedEcoTag.equals(HIGHLY_POISONOUS)) {
                if (!appliedFilterList.contains(BadgesIconVM.FrostBadges.get(frostBadgeNumber).getName()))
                    appliedFilterList.add(BadgesIconVM.FrostBadges.get(frostBadgeNumber).getName());
            } else if (!appliedFilterList.contains(BadgesIconVM.EcoBadges.get(ecoBadgeNumber).getName()))
                appliedFilterList.add(BadgesIconVM.EcoBadges.get(ecoBadgeNumber).getName());
            _binding.textViewMyGardenAppliedFilter.setVisibility(View.VISIBLE);
        }
        String filters = "";
        for (String filter : appliedFilterList) {
            filters += filter + "\n";
        }
        _binding.textViewMyGardenAppliedFilter.setText((filters).trim());

        setEcoCounters(myGardenList);

        Log.d("ApplieffilterList", "bind: " + appliedFilterList);
        setSelectedTagBackground(_binding);


    }

    public void setEcoCounters(List<MyGarden> myGardenList) {
        this.myGardenList = myGardenList;
        long count = 0L;
        for (MyGarden myGarden : myGardenList) {
            for (Badge badge : myGarden.getUserPlant().getBadges()) {
                if (badge.getId().equals(BEE_FRIENDLY_KEY)) {
                    count += myGarden.getUserPlant().getCount();
                }
            }
        }
        this.beeFriendlyCount = count;

        long count1 = 0L;
        for (MyGarden garden : myGardenList) {
            for (Badge p1 : garden.getUserPlant().getBadges()) {
                if (p1.getId().equals(BIRD_FRIENDLY_KEY_1) || p1.getId().equals(BIRD_FRIENDLY_KEY_2)) {
                    count1 += garden.getUserPlant().getCount();
                }
            }
        }
        this.birdFriendlyCount = count1;

        long result = 0L;
        for (MyGarden myGarden : myGardenList) {
            for (Badge badge : myGarden.getUserPlant().getBadges()) {
                if (badge.getId().equals(INSECT_FRIENDLY_KEY)) {
                    result += myGarden.getUserPlant().getCount();
                }
            }
        }
        this.insectFriendlyCount = result;

        long result1 = 0L;
        for (MyGarden myGarden : myGardenList) {
            for (Badge badge : myGarden.getUserPlant().getBadges()) {
                if (badge.getId().equals(ECOLOGICAL_PLANT_KEY)) {
                    result1 += myGarden.getUserPlant().getCount();
                }
            }
        }
        this.ecologicalCount = result1;

        long count2 = 0L;
        for (MyGarden myGarden : myGardenList) {
            for (Badge badge : myGarden.getUserPlant().getBadges()) {
                if (badge.getId().equals(BUTTERFLY_FRIENDLY_KEY)) {
                    count2 += myGarden.getUserPlant().getCount();
                }
            }
        }
        this.butterflyFriendlyCount = count2;

        long result2 = 0L;
        for (MyGarden myGarden : myGardenList) {
            for (Badge badge : myGarden.getUserPlant().getBadges()) {
                if (badge.getId().equals(NATIVE_PLANT_KEY)) {
                    result2 += myGarden.getUserPlant().getCount();
                }
            }
        }
        this.nativeCount = result2;

        long count3 = 0L;
        for (MyGarden a : myGardenList) {
            for (Badge p : a.getUserPlant().getBadges()) {
                if (p.getId().equals(WATER_SAVING_PLANT_KEY)) {
                    count3 += a.getUserPlant().getCount();
                }
            }
        }
        this.waterSavingCount = count3;
    }

    private void setSelectedTagBackground(@NonNull com.gardify.android.databinding.RecyclerViewMyGardenPlantFilterIconsItemBinding _binding) {
        _binding.llBeeFriendly.setBackgroundColor(Color.TRANSPARENT);
        _binding.llBirdFriendly.setBackgroundColor(Color.TRANSPARENT);
        _binding.llInsectFriendly.setBackgroundColor(Color.TRANSPARENT);
        _binding.llEcologicalPlant.setBackgroundColor(Color.TRANSPARENT);
        _binding.llButterflyFriendly.setBackgroundColor(Color.TRANSPARENT);
        _binding.llNativePlant.setBackgroundColor(Color.TRANSPARENT);
        _binding.llWaterSaving.setBackgroundColor(Color.TRANSPARENT);


        //setting background
        if (selectedEcoTag.equals(BEE_FRIENDLY_KEY)) {
            _binding.llBeeFriendly.setBackgroundColor(Color.WHITE);
            ecoBadgeNumber = 0;
        } else if (selectedEcoTag.equals(BIRD_FRIENDLY_KEY_1) || selectedEcoTag.equals(BIRD_FRIENDLY_KEY_2)) {
            _binding.llBirdFriendly.setBackgroundColor(Color.WHITE);
            ecoBadgeNumber = 4;
        } else if (selectedEcoTag.equals(INSECT_FRIENDLY_KEY)) {
            _binding.llInsectFriendly.setBackgroundColor(Color.WHITE);
            ecoBadgeNumber = 1;
        } else if (selectedEcoTag.equals(ECOLOGICAL_PLANT_KEY)) {
            _binding.llEcologicalPlant.setBackgroundColor(Color.WHITE);
            ecoBadgeNumber = 3;
        } else if (selectedEcoTag.equals(BUTTERFLY_FRIENDLY_KEY)) {
            _binding.llButterflyFriendly.setBackgroundColor(Color.WHITE);
            ecoBadgeNumber = 5;
        } else if (selectedEcoTag.equals(NATIVE_PLANT_KEY)) {
            _binding.llNativePlant.setBackgroundColor(Color.WHITE);
            ecoBadgeNumber = 2;
        } else if (selectedEcoTag.equals(WATER_SAVING_PLANT_KEY)) {
            _binding.llWaterSaving.setBackgroundColor(Color.WHITE);
            ecoBadgeNumber = 6;
        }
        if (selectedEcoTag.equals(NOT_FROST_RESISTANT))
            frostBadgeNumber = 0;
        else if (selectedEcoTag.equals(FROST_MINUS_FIVE))
            frostBadgeNumber = 1;
        else if (selectedEcoTag.equals(FROST_MINUS_TEN))
            frostBadgeNumber = 2;
        else if (selectedEcoTag.equals(COMPLETELY_FROST_RESISTANT))
            frostBadgeNumber = 3;
        else if (selectedEcoTag.equals(CONDITIONALLY_POISONOUS))
            frostBadgeNumber = 4;
        else if (selectedEcoTag.equals(HIGHLY_POISONOUS))
            frostBadgeNumber = 5;
    }

    public void setText(CharSequence text) {
        this.text = text;
    }

    public void selectEcoTag(String ecoTag) {
        this.selectedEcoTag = ecoTag;
    }

    public CharSequence getText() {
        return text;
    }
}
