package com.gardify.android.ui.myGarden.recyclerItems;

import android.app.Activity;
import android.content.Context;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ImageView;
import android.widget.LinearLayout;

import androidx.annotation.NonNull;
import androidx.fragment.app.FragmentActivity;
import androidx.recyclerview.widget.GridLayoutManager;

import com.gardify.android.R;
import com.gardify.android.data.myGarden.Badge;
import com.gardify.android.data.myGarden.MyGarden;
import com.gardify.android.data.myGarden.UserPlant;
import com.gardify.android.databinding.ItemGardenCardBinding;
import com.gardify.android.generic.CustomBottomSheet;
import com.gardify.android.ui.myGarden.interfaces.OnPlantClickListener;
import com.gardify.android.ui.myGarden.MyGardenFragment;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.ApiUtils;
import com.gardify.android.utils.TimeUtils;
import com.gardify.android.viewModelData.BadgesIconVM;
import com.xwray.groupie.GroupAdapter;
import com.xwray.groupie.Section;
import com.xwray.groupie.databinding.BindableItem;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

import static com.gardify.android.utils.CollectionUtils.removeDuplicates;
import static com.gardify.android.utils.StringUtils.formatHtmlKTags;
import static com.gardify.android.utils.UiUtils.loadImageUsingGlide;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.viewModelData.BadgesIconVM.EcoBadges;

public class GardenCardItem extends BindableItem<ItemGardenCardBinding> {

    public static final String FAVORITE = "FAVORITE";
    private static final String TAG = "MyGarden GardenCardItem: ";
    private static final String ARG_MY_GARDEN_TODO = "MY_GARDEN_TODO";

    private OnPlantClickListener onPlantClickListener;
    private MyGarden myGarden;
    private Context context;
    private Activity activity;

    public GardenCardItem(long id, Context context, MyGarden myGarden, OnPlantClickListener onPlantClickListener) {
        super(id);
        this.onPlantClickListener = onPlantClickListener;
        this.myGarden = myGarden;
        this.context = context;
        activity = (Activity) context;
        getExtras().put(MyGardenFragment.INSET_TYPE_KEY, MyGardenFragment.INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.item_garden_card;
    }

    @Override
    public void bind(@NonNull final ItemGardenCardBinding binding, int position) {

        String imageUrl = APP_URL.BASE_ROUTE_INTERN + myGarden.getUserPlant().getImages().get(0).getSrcAttr();
        loadImageUsingGlide(context, imageUrl, binding.imageViewMyGardenPlantItemPlantImage);

        binding.tvMyGardenPlantNameLatin.setText(formatHtmlKTags(myGarden.getUserPlant().getNameLatin()));
        binding.tvMyGardenPlantName.setText(myGarden.getUserPlant().getName());

        if (myGarden.getUserPlant().getSynonym() != null && myGarden.getUserPlant().getSynonym().length() > 2)
            binding.tvMyGardenPlantNameSynonyms.setText(formatHtmlKTags(myGarden.getUserPlant().getSynonym()));
        else
            binding.tvMyGardenPlantNameSynonyms.setVisibility(View.GONE);

        String gardenListNames = TextUtils.join(", ", myGarden.getListNames());

        if (myGarden.getUserPlant().getIsInPot()) {
            binding.tvMyGardenIsInPot.setText(String.format("Liste: " + gardenListNames + ", Topfpflanze"));

        } else {
            binding.tvMyGardenIsInPot.setText(String.format("Liste: " + gardenListNames + ", Keine Topfpflanze"));
        }

        displayPlantNote(binding);

        binding.buttonItemGardenCardDetail.setOnClickListener(v -> onPlantClickListener.onClick(myGarden, binding, null, binding.buttonItemGardenCardDetail, position));
        binding.imageViewMyGardenPlantItemPlantImage.setOnClickListener(v -> onPlantClickListener.onClick(myGarden, binding, null, binding.imageViewMyGardenPlantItemPlantImage, position));

        binding.textviewMyGardenPlantOptions.setOnClickListener(v -> onPlantClickListener.onClick(myGarden, binding, null, binding.textviewMyGardenPlantOptions, position));
        binding.tvMyGardenTodoDone.setOnClickListener(v -> onPlantClickListener.onClick(myGarden, binding, null, binding.textviewMyGardenPlantOptions, position));

        // show ökologische Kriterien badges
        LayoutInflater inflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        showEcoBadges(binding, inflater);

        // show todos
        showTodoRecycleView(binding);

    }

    private void displayPlantNote(@NonNull com.gardify.android.databinding.ItemGardenCardBinding binding) {
        String numberOfPlantsAdded;
        if (myGarden.getUserPlant().getCount() > 1) {
            numberOfPlantsAdded = myGarden.getUserPlant().getCount() +" Pflanzen eingepflegt am";
        } else {
            numberOfPlantsAdded = myGarden.getUserPlant().getCount() + " Pflanze eingepflegt am";
        }
        String plantNote = myGarden.getUserPlant().getNotes() != null ? "-" +myGarden.getUserPlant().getNotes() : "";
        String plantAddedDate = TimeUtils.dateToString(myGarden.getUserPlant().getDatePlanted(), "yyyy-MM-dd'T'HH:mm:ss.SSS", "dd.MM.yyyy");

        binding.tvMyGardenPlantDateAndNotes.setText(numberOfPlantsAdded + " "+plantAddedDate+ " " + plantNote);
    }

    private void showEcoBadges(@NonNull com.gardify.android.databinding.ItemGardenCardBinding binding, LayoutInflater inflater) {
        LinearLayout containerBio = binding.linearLayoutBioIcons;
        containerBio.removeAllViews();
        View childViewBio;

        List<BadgesIconVM> EcoBadgesList = EcoBadges;
        List<BadgesIconVM> plantBadgesList = new ArrayList<>();

        for (Badge badge : myGarden.getUserPlant().getBadges()) {
            Optional<BadgesIconVM> matchingObject = EcoBadgesList.stream().
                    filter(p -> p.getId().contains(String.valueOf(badge.getId()))).
                    findFirst();
            if (matchingObject.isPresent()) {
                BadgesIconVM badgesIconVM = matchingObject.get();
                plantBadgesList.add(badgesIconVM);
            }
        }

        // remove duplicate badge icons
        plantBadgesList = removeDuplicates(plantBadgesList);

        // inside loop
        for (BadgesIconVM badgesIcon : plantBadgesList) {
            childViewBio = inflater.inflate(R.layout.view_plant_bio_icon, null);
            ImageView imageView = childViewBio.findViewById(R.id.plant_bio_icon);
            int resId = getResId(badgesIcon);
            imageView.setBackground(context.getResources().getDrawable(resId, null));
            containerBio.addView(childViewBio);
        }
    }

    private void showTodoRecycleView(@NonNull ItemGardenCardBinding binding) {
        GroupAdapter todoAdapter = new GroupAdapter();
        GridLayoutManager layoutManager = new GridLayoutManager(context, todoAdapter.getSpanCount());
        layoutManager.setSpanSizeLookup(todoAdapter.getSpanSizeLookup());
        binding.todoMygardenRecycleview.setLayoutManager(layoutManager);
        Section todoSection = new Section();
        int todoListSize = myGarden.getUserPlant().getCyclicTodos() != null ? myGarden.getUserPlant().getCyclicTodos().size() : 0;

        for (int i = 0; i < todoListSize; i++) {
            todoSection.add(new CardTodoItem(context, R.color.section_myGarden_toDos, myGarden, onCardTodoListener));
        }
        todoAdapter.add(todoSection);

        binding.todoMygardenRecycleview.setAdapter(todoAdapter);

        // show no todos text if todolist is empty
        if (todoListSize == 0) {
            binding.textviewMyGardenTodoStatus.setVisibility(View.VISIBLE);
            binding.textviewMyGardenTodoStatus.setText("Es gibt keine aktiven To-Dos");
        } else {
            binding.textviewMyGardenTodoStatus.setVisibility(View.GONE);
            //show bottomsheet onTodos options click
            binding.tvMyGardenTodoOptions.setOnClickListener(v -> {
                showBottomSheet(null);
            });
        }
    }

    private void showBottomSheet(UserPlant.CyclicTodo cyclicTodo) {
        CustomBottomSheet customBottomSheet = new CustomBottomSheet(R.string.all_delete, cyclicTodo, "Texte ändern, löschen und als erledigt markieren kannst du im To-Do Kalender",
                "zum To-Do Kalender", onBottomSheetClickListener);
        customBottomSheet.show(((FragmentActivity) activity).getSupportFragmentManager(), "BottomSheet");
    }

    private int getResId(BadgesIconVM badgesIconVM) {

        return context.getResources().getIdentifier("gardify_app_icon_" + badgesIconVM.getImage().toLowerCase(), "drawable", context.getApplicationInfo().packageName);
    }

    private CustomBottomSheet.OnBottomSheetClickListener onBottomSheetClickListener = (stringId, cyclicTodo) -> {
        Bundle bundle = null;
        if (cyclicTodo != null) {
            bundle = new Bundle();
            String todoModelJsonString = ApiUtils.getGsonParser().toJson(cyclicTodo);
            bundle.putString("MY_GARDEN_TODO", todoModelJsonString);
        }
        navigateToFragment(R.id.nav_controller_todo, activity, false, bundle);
    };

    private CardTodoItem.OnTodoCardClickListener onCardTodoListener = (viewbinding, view, cyclicTodo, todoId) -> {

        if (viewbinding.myGardenTodoExpandIcon.equals(view) || viewbinding.myGardenTodoText.equals(view)) {
            showBottomSheet(cyclicTodo);
        }
    };

}
