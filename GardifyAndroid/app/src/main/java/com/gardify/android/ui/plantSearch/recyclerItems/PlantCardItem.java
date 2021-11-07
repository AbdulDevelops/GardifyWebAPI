package com.gardify.android.ui.plantSearch.recyclerItems;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ImageView;
import android.widget.LinearLayout;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.data.plantSearchModel.Badge;
import com.gardify.android.data.plantSearchModel.Plant;
import com.gardify.android.data.shared.DataHelper;
import com.gardify.android.databinding.RecyclerViewPlantSearchPlantItemBinding;
import com.gardify.android.ui.myGarden.MyGardenFragment;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.viewModelData.BadgesIconVM;
import com.xwray.groupie.databinding.BindableItem;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

import static com.gardify.android.utils.CollectionUtils.removeDuplicates;
import static com.gardify.android.utils.CollectionUtils.safeCheckList;
import static com.gardify.android.utils.StringUtils.formatHtmlKTags;
import static com.gardify.android.utils.UiUtils.loadImageUsingGlide;
import static com.gardify.android.viewModelData.BadgesIconVM.EcoBadges;

public class PlantCardItem extends BindableItem<RecyclerViewPlantSearchPlantItemBinding> {

    public static final String FAVORITE = "FAVORITE";

    private OnPlantClickListener onPlantClickListener;
    private boolean checked = false;
    private Plant plant;
    private Context context;
    private boolean inUserGarden;
    private boolean expand = false;
    private List<BadgesIconVM> badgesIconVMList = EcoBadges;

    public PlantCardItem(long id, Context context, Plant plant, boolean inUserGarden, OnPlantClickListener onPlantClickListener) {
        super(id);
        this.onPlantClickListener = onPlantClickListener;
        this.plant = plant;
        this.context = context;
        this.inUserGarden = inUserGarden;
        getExtras().put(MyGardenFragment.INSET_TYPE_KEY, MyGardenFragment.INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_plant_search_plant_item;
    }

    @Override
    public void bind(@NonNull final RecyclerViewPlantSearchPlantItemBinding binding, int position) {

        adjustDescriptionMaxLines(binding);

        binding.tvPlantSearchPlantNameLatin.setText(formatHtmlKTags(plant.getNameLatin()));
        binding.tvPlantSearchPlantName.setText(plant.getNameGerman());

        if (plant.getSynonym() != null && plant.getSynonym().length() > 2)
            binding.tvPlantSearchPlantNameSynonym.setText(formatHtmlKTags(plant.getSynonym()));
        else
            binding.tvPlantSearchPlantNameSynonym.setVisibility(View.GONE);

        binding.tvPlantSearchDescription.setText(formatHtmlKTags(plant.getDescription()));

        String imageUrl = APP_URL.BASE_ROUTE_INTERN + plant.getImage().get(0).getSrcAttr();
        loadImageUsingGlide(context, imageUrl, binding.imageViewPlantSearchPlantImage);

        //linear layout
        LayoutInflater inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);

        showColorBadge(binding, inflater);

        //linear layout2
        showEcoBadges(binding, inflater);

        if(inUserGarden)
            binding.buttonPlantSearchSaveToGarden.setText(R.string.plantSearch_moveToAnotherGarden);

        binding.buttonRecyclerViewPLantSearchPlantItemDetail.setOnClickListener(v -> onPlantClickListener.onClick(plant, binding, binding.buttonRecyclerViewPLantSearchPlantItemDetail, position));
        binding.imageViewPlantSearchPlantImage.setOnClickListener(v -> onPlantClickListener.onClick(plant, binding, binding.imageViewPlantSearchPlantImage, position));
        binding.buttonPlantSearchSaveToGarden.setOnClickListener(v -> onPlantClickListener.onClick(plant, binding, binding.buttonPlantSearchSaveToGarden, position));

    }

    private void adjustDescriptionMaxLines(@NonNull com.gardify.android.databinding.RecyclerViewPlantSearchPlantItemBinding binding) {
        binding.expandIconDescription.setOnClickListener(v -> {
            if (!expand) {
                binding.expandIconDescription.setImageResource(R.drawable.collapse);
                binding.tvPlantSearchDescription.setMaxLines(50);
                expand = true;
            } else {
                binding.expandIconDescription.setImageResource(R.drawable.expand);
                binding.tvPlantSearchDescription.setMaxLines(4);
                expand = false;
            }

        });
    }

    private void showColorBadge(@NonNull RecyclerViewPlantSearchPlantItemBinding binding, LayoutInflater inflater) {
        LinearLayout containerColor = binding.linearLayoutFlowerColor;
        containerColor.removeAllViews();
        View childViewColor;
        for (Object colorName : safeCheckList(plant.getColors())) {
            childViewColor = inflater.inflate(R.layout.view_plant_color_image, null);
            ImageView imageView = childViewColor.findViewById(R.id.plant_color_image);
            Integer colorValue = DataHelper.colorsMap.get(colorName);
            imageView.setBackgroundColor(colorValue);

            containerColor.addView(childViewColor);
        }
    }

    private void showEcoBadges(@NonNull com.gardify.android.databinding.RecyclerViewPlantSearchPlantItemBinding binding, LayoutInflater inflater) {
        LinearLayout containerBio = binding.linearLayoutBioIcons;
        containerBio.removeAllViews();
        View childViewBio;
        //TODO wasser_pflanzen badge icon not showing from api (e.g plant dreiblättrige)
        List<BadgesIconVM> plantBadgesList = new ArrayList<>();
        for (Badge badge : plant.getBadge()) {
            Optional<BadgesIconVM> matchingObject = badgesIconVMList.stream().
                    filter(p -> p.getId().contains(String.valueOf(badge.getId()))).
                    findFirst();
            if (matchingObject.isPresent()) {
                BadgesIconVM badgesIconVM = matchingObject.get();
                plantBadgesList.add(badgesIconVM);
            }
        }

        // remove duplicate badge icons
        plantBadgesList = removeDuplicates(plantBadgesList);

        //inflate badge icons
        for (BadgesIconVM badgesIcon : plantBadgesList) {

            childViewBio = inflater.inflate(R.layout.view_plant_bio_icon, null);
            ImageView imageView = childViewBio.findViewById(R.id.plant_bio_icon);
            int resId = getResId(badgesIcon);
            imageView.setBackground(context.getResources().getDrawable(resId, null));
            containerBio.addView(childViewBio);
        }
    }

    public interface OnPlantClickListener {
        void onClick(Plant plant, RecyclerViewPlantSearchPlantItemBinding viewBinding, View view, int Pos);
    }

    private int getResId(BadgesIconVM badgesIconVM) {

        return context.getResources().getIdentifier("gardify_app_icon_" + badgesIconVM.getImage().toLowerCase(), "drawable", context.getApplicationInfo().packageName);
    }
}
