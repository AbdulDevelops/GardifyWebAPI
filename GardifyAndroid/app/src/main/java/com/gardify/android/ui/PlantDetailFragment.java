package com.gardify.android.ui;

import android.content.Context;
import android.content.res.Resources;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.GridLayout;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.TextView;

import androidx.fragment.app.Fragment;

import com.gardify.android.data.general.SimilarPlant;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.plantDetail.PlantDetail;
import com.gardify.android.R;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.gardify.android.utils.StringUtils;

import org.jetbrains.annotations.NotNull;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

import static com.gardify.android.ui.saveToGarden.SaveToGardenFragment.PLANT_ID_ARG;
import static com.gardify.android.ui.saveToGarden.SaveToGardenFragment.PLANT_NAME_ARG;
import static com.gardify.android.utils.StringUtils.formatHtmlKTags;
import static com.gardify.android.utils.UiUtils.loadImageUsingGlide;
import static com.gardify.android.utils.UiUtils.navigateToFragment;


public class PlantDetailFragment extends Fragment implements View.OnClickListener {

    private static final String TAG = "PlantDetailFragment";

    private String plantId, plantName;
    private TextView plantNameLatinTv, plantNameGermanTv, plantSynonymTv, plantGroupTv, plantFamilyTv, plantHerkunftTv, plantDescTv;
    private Button saveToGardenBtn;
    private ImageView mainPlantImage;
    private ProgressBar progressBar;
    private LinearLayout linearLayoutCategoryTable, linearLayoutTodos;
    private GridLayout gridLayoutSimilarPlants;
    private TextView plantGroupLabel, plantFamilyLabel, plantOriginLabel, descriptionLabel, similarPlantHeader;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (getArguments() != null) {
            plantId = getArguments().getString(PLANT_ID_ARG);
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_plant_detail, container, false);

        init(view);

        String apiUrl = APP_URL.PLANT_SEARCH + plantId;
        RequestQueueSingleton.getInstance(getContext()).typedRequest(apiUrl, this::onSuccess, this::onError, PlantDetail.class, new RequestData(RequestType.PlantDetail));

        return view;
    }

    public void init(View view) {
        /* finding views block */
        plantNameLatinTv = view.findViewById(R.id.textView_plants_details_name_latin);
        plantNameGermanTv = view.findViewById(R.id.textView_plants_details_name_german);
        plantSynonymTv = view.findViewById(R.id.textView_plants_details_name_synonym);
        plantGroupTv = view.findViewById(R.id.textView_plants_details_group_name);
        plantFamilyTv = view.findViewById(R.id.textView_plants_details_family_name);
        plantHerkunftTv = view.findViewById(R.id.textView_plants_details_origin_name);
        plantDescTv = view.findViewById(R.id.textView_plants_details_description);
        mainPlantImage = view.findViewById(R.id.imageView_plants_details_plant_image);
        progressBar = view.findViewById(R.id.progressbar);
        linearLayoutCategoryTable = view.findViewById(R.id.linear_layout_plant_detail_category_table);
        linearLayoutTodos = view.findViewById(R.id.linear_layout_plant_detail_todo);
        saveToGardenBtn = view.findViewById(R.id.button_plants_details_save_garden);
        gridLayoutSimilarPlants = view.findViewById(R.id.grid_layout_plant_detail);

        // Labels
        plantGroupLabel = view.findViewById(R.id.textView_plants_details_group_label);
        plantFamilyLabel = view.findViewById(R.id.textView_plants_details_family_label);
        plantOriginLabel = view.findViewById(R.id.textView_plants_details_origin_label);
        descriptionLabel = view.findViewById(R.id.textView_plants_details_description_label);
        similarPlantHeader = view.findViewById(R.id.textView_detail_similar_plant_header);

        //hide labels
        uiLabelVisibility(View.INVISIBLE);

        /* setting views onclick listeners block */
        saveToGardenBtn.setOnClickListener(this);
    }

    private void uiLabelVisibility(int visibility) {
        plantGroupLabel.setVisibility(visibility);
        plantFamilyLabel.setVisibility(visibility);
        plantOriginLabel.setVisibility(visibility);
        descriptionLabel.setVisibility(visibility);
        saveToGardenBtn.setVisibility(visibility);
    }

    private void onSuccess(PlantDetail model, RequestData data) {

        //show Labels
        uiLabelVisibility(View.VISIBLE);

        // set text and data
        plantNameLatinTv.setText(StringUtils.formatHtmlKTags(model.getNameLatin()));
        plantNameGermanTv.setText(model.getNameGerman());
        plantName = model.getNameGerman();
        if (model.getSynonym() != null && model.getSynonym().length() > 2)
            plantSynonymTv.setText(formatHtmlKTags(model.getSynonym()));
        else
            plantSynonymTv.setVisibility(View.GONE);

        progressBar.setVisibility(View.GONE);
        plantGroupTv.setText(groupsCommaSeparated(model));
        plantFamilyTv.setText(model.getFamily());
        plantHerkunftTv.setText(model.getHerkunft());
        plantDescTv.setText(StringUtils.formatHtmlKTags(model.getDescription()));

        String imageUrl = APP_URL.BASE_ROUTE_INTERN + model.getImages().get(0).getSrcAttr();
        loadImageUsingGlide(getContext(), imageUrl, mainPlantImage);

        showTableData(model);

        showTodoData(model);
    }

    private void showTodoData(PlantDetail model) {
        LayoutInflater inflater = (LayoutInflater) getActivity().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        View detailRowView;
        TextView textViewTodoDesc, textViewTodoHeader, textViewTodoName;

        // header
        detailRowView = inflater.inflate(R.layout.view_plant_detail_todo_row, null);
        textViewTodoName = detailRowView.findViewById(R.id.text_view_detail_todo_name);
        textViewTodoDesc = detailRowView.findViewById(R.id.text_view_detail_todo_description);
        // show only header and hide tags
        textViewTodoName.setVisibility(View.GONE);
        textViewTodoDesc.setVisibility(View.GONE);
        linearLayoutTodos.addView(detailRowView);

        for (PlantDetail.TodoTemplate todoTemplate : model.getTodoTemplates()) {

            // display category items
            detailRowView = inflater.inflate(R.layout.view_plant_detail_todo_row, null);
            textViewTodoHeader = detailRowView.findViewById(R.id.text_view_detail_todo_header);
            textViewTodoHeader.setVisibility(View.GONE);
            textViewTodoName = detailRowView.findViewById(R.id.text_view_detail_todo_name);
            textViewTodoDesc = detailRowView.findViewById(R.id.text_view_detail_todo_description);
            textViewTodoName.setText(todoTemplate.getTitle());
            textViewTodoDesc.setText(todoTemplate.getDescription());
            linearLayoutTodos.addView(detailRowView);
        }
    }

    private void showTableData(PlantDetail model) {

        PlantDetailData plantDetailData = new PlantDetailData();

        String[] tableDataHeadingList = plantDetailData.headingList;
        String[][] tableDataSectionCategoryList = plantDetailData.sectionCategoryList;
        Map<String, Integer> categoryIdMap = plantDetailData.categoryIdMap;

        LayoutInflater inflater = (LayoutInflater) getActivity().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        View detailRowView;
        TextView textViewCategoryHeader, textViewCategoryName, textViewCategoryTags;
        LinearLayout linearLayoutCategoryTags, linearLayoutHeader;
        for (int i = 0; i < tableDataHeadingList.length; i++) {
            // header
            detailRowView = inflater.inflate(R.layout.view_plant_detail_category_row, null);
            textViewCategoryHeader = detailRowView.findViewById(R.id.text_view_detail_category_header);
            linearLayoutCategoryTags = detailRowView.findViewById(R.id.linear_layout_detail_category_tags);
            // show only header and hide tags
            textViewCategoryHeader.setText(tableDataHeadingList[i]);
            linearLayoutCategoryTags.setVisibility(View.GONE);
            linearLayoutCategoryTable.addView(detailRowView);

            int empty = 0;
            Log.d(TAG, "showTableData: Empty reset!");
            // sectionCategoryList
            for (int j = 0; j < tableDataSectionCategoryList[i].length; j++) {
                String categoryName = tableDataSectionCategoryList[i][j];

                // get tags matching category name only
                List<String> getCategoryTagsList = model.getPlantTags().stream()
                        .filter(p -> p.getCategoryId() == categoryIdMap.get(categoryName))
                        // get only string attribute of complete PlantDetail.PlantTagsEntity
                        .map(PlantDetail.PlantTag::getTitle)
                        .collect(Collectors.toList());

                // convert string list to comma separated string
                String tagsInString = getCategoryTagsList.stream().collect(Collectors.joining(", "));
                //brauche die Anzahl an Category tags in einem CategoryName, um zu wissen, ob alle Leer sind
                // display category items
                detailRowView = inflater.inflate(R.layout.view_plant_detail_category_row, null);
                linearLayoutHeader = detailRowView.findViewById(R.id.linear_layout_detail_header);
                linearLayoutHeader.setVisibility(View.GONE);
                textViewCategoryName = detailRowView.findViewById(R.id.text_view_detail_category_name);
                textViewCategoryTags = detailRowView.findViewById(R.id.text_view_detail_category_tags);
                if (tagsInString.isEmpty()) {
                        textViewCategoryName.setVisibility(View.GONE);
                        textViewCategoryTags.setVisibility(View.GONE);
                        empty++;
                } else {
                    textViewCategoryName.setText(categoryName);
                    textViewCategoryTags.setText(tagsInString);
                }
                Log.d(TAG, "showTableData: categoryTagsTextView: " + categoryName + ": " + tagsInString +
                        //+ "\nsectioncategoryList[i]: " + Arrays.toString(plantDetailData.sectionCategoryList[i]) +
                        "\nsectionCategoryList.length: " + plantDetailData.sectionCategoryList.length
                        + "\nEmpty: " + empty);
                linearLayoutCategoryTable.addView(detailRowView);
            }
            Log.d(TAG, "showTableData: Emptyy" + empty);
        }

        getSimilarPlant(model);

    }

    private void getSimilarPlant(PlantDetail model) {

        String similarPlantUrl = APP_URL.PLANT_SEARCH + "findSiblingById/" + model.getId();
        RequestQueueSingleton.getInstance(getContext()).typedRequest(similarPlantUrl,
                this::onSuccessSimilarPlant, this::onError, SimilarPlant[].class,
                new RequestData(RequestType.SimilarPlant));

    }

    private void onSuccessSimilarPlant(SimilarPlant[] similarPlants, RequestData data) {
        SimilarPlant[] similarPlantsList = similarPlants;
        Log.d(TAG, "onSuccessSimilarPlant: " + Arrays.asList(similarPlantsList));
        // display images
        LayoutInflater inflater = (LayoutInflater) getActivity().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        View detailRowView;
        ImageView similarImage;

        for (SimilarPlant similarPlant : similarPlantsList) {
            Log.d(TAG, "onSuccessSimilarPlant: " + similarPlantsList.length);

            detailRowView = inflater.inflate(R.layout.view_plant_detail_similar_plant_row, gridLayoutSimilarPlants, false);

            similarImage = detailRowView.findViewById(R.id.image_view_detail_similar_image);
            String imageUrl = APP_URL.BASE_ROUTE_INTERN + similarPlant.getImages().get(0).getSrcAttr();
            loadImageUsingGlide(getContext(), imageUrl, similarImage);

            similarImage.setOnClickListener(v -> {
                Bundle bundle = new Bundle();
                bundle.putString(PLANT_ID_ARG, String.valueOf(similarPlant.getId()));
                navigateToFragment(R.id.nav_controller_plant_detail, getActivity(), false, bundle);
            });

            gridLayoutSimilarPlants.addView(detailRowView);

        }
    }

    @NotNull
    private String groupsCommaSeparated(PlantDetail model) {
        return model.getPlantGroups()
                .stream()
                .map(c -> String.valueOf(c.getName()))
                .collect(Collectors.joining(", "));
    }

    public void onError(Exception ex, RequestData data) {
        if (isVisible()) {
            Resources res = getContext().getResources();
            String network = res.getString(R.string.all_network);
            String anErrorOccurred = res.getString(R.string.all_anErrorOccurred);
            Log.e(network, anErrorOccurred + ex.toString());
        }
    }

    @Override
    public void onClick(View view) {

        switch (view.getId()) {

            case R.id.button_plants_details_save_garden:

                // Change fragment
                Bundle bundle = new Bundle();
                bundle.putString(PLANT_ID_ARG, plantId);
                bundle.putString(PLANT_NAME_ARG, String.valueOf(plantName));
                navigateToFragment(R.id.nav_controller_save_to_garden, getActivity(), false, bundle);
                break;

        }
    }

    private class PlantDetailData {
        String[] headingList = {
                "Eigenschaften",
                "Blüten",
                "Standort & Pflege",
                "Blätter",
                "Früchte"};

        String[][] sectionCategoryList = {
                {"Verwendung", "Besonderheiten", "Wuchs", "Nutzpflanzen", "Laubrhythmus", "Winterhärte"},
                {"Blüten", "Blütenfarben", "Blütenform", "Blütengröße", "Blütenstand"},
                {"Licht", "Boden", "Schnitt", "Wasserbedarf", "Vermehrung"},
                {"Blattfarbe", "Blattform", "Blattrand", "Blattstellung"},
                {"Früchte"}
        };


        Map<String, Integer> categoryIdMap = new HashMap<String, Integer>() {{
            put("Besonderheiten", 108);
            put("Ausschlusskriterien", 128);
            put("Dekoaspekte", 121);
            put("Verwendung", 97);
            put("Vermehrung", 95);
            put("Düngung", 94);
            put("Schnitt", 92);
            put("Wasserbedarf", 91);
            put("Winter Hardness", 89);
            put("Boden", 87);
            put("Licht", 86);
            put("Wuchs", 74);
            put("Nutzpflanzen", 137);
            put("Fruchtfarbe", 71);
            put("Früchte", 68);
            put("Blütengröße", 67);
            put("Blütenstand", 66);
            put("Blütenform", 65);
            put("Blütenfarben", 64);
            put("Laubrhythmus", 53);
            put("Blattfarbe", 55);
            put("Herbstfärbung", 56);
            put("Blattform", 57);
            put("Blattrand", 58);
            put("Blattstellung", 59);
            put("Blüten", 60);
            put("Winterhärte", 89);
        }};
    }
}