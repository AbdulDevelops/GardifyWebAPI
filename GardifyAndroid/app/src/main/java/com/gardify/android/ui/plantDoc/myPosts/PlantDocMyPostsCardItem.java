package com.gardify.android.ui.plantDoc.myPosts;

import android.app.Activity;
import android.content.Context;
import android.os.Bundle;
import android.view.View;

import androidx.annotation.NonNull;

import com.gardify.android.data.plantsDocModel.Image;
import com.gardify.android.data.plantsDocModel.PlantDocViewModel;
import com.gardify.android.R;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.databinding.RecyclerViewPlantDocMyPostFilterItemBinding;
import com.xwray.groupie.databinding.BindableItem;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.List;

import static com.gardify.android.utils.UiUtils.loadImageUsingGlide;
import static com.gardify.android.utils.UiUtils.navigateToFragment;


public class PlantDocMyPostsCardItem extends BindableItem<RecyclerViewPlantDocMyPostFilterItemBinding> {
    private CharSequence text;
    private Context context;
    private PlantDocViewModel plantDocModel;
    private boolean isInfoVisible;

    public PlantDocMyPostsCardItem(Context context, PlantDocViewModel plantDocModel, CharSequence text, boolean isInfoVisible) {
        this.context=context;
        this.plantDocModel=plantDocModel;
        this.text = text;
        this.isInfoVisible=isInfoVisible;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_plant_doc_my_post_filter_item;
    }

    @Override
    public void bind(@NonNull RecyclerViewPlantDocMyPostFilterItemBinding viewBinding, int position) {
        List<Image> images = plantDocModel.getImages();
        String imageUrl = APP_URL.BASE_ROUTE + images.get(0).getSrcAttr();
        loadImageUsingGlide(context, imageUrl, viewBinding.imageViewPlantDocMyPostsPlantImage);

        //TODO list of images in slider
        for (Image img : images) {
        }

        SimpleDateFormat inFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS");
        SimpleDateFormat outFormat = new SimpleDateFormat("dd.MM.yyyy");
        Date d = null;
        try {
            d = inFormat.parse(plantDocModel.getPublishDate());
        } catch (ParseException e) {
            e.printStackTrace();
        }

        String formattedDate = outFormat.format(d);
        //Glide.with(context).load(imageURl).centerCrop().into(viewBinding.imageViewGardenKnowledgePlantFilterIcon);
        viewBinding.textViewPlantDocMyPostFilterItemDate.setText(formattedDate);
        viewBinding.textViewPlantDocMyPostFilterItemTheme.setText(plantDocModel.getThema());
        if(plantDocModel.getTotalAnswers() < 2){
            viewBinding.textViewPlantDocMyPostsPlantFilterAnswer.setText(plantDocModel.getTotalAnswers().toString() + " Antwort");
        }else {
            viewBinding.textViewPlantDocMyPostsPlantFilterAnswer.setText(plantDocModel.getTotalAnswers().toString() + " Antworten");
        }
        viewBinding.imageButtonPlantDocMyPostsPlantFilterIcon.setImageResource(R.drawable.expand);
        viewBinding.imageButtonPlantDocMyPostsPlantFilterIcon.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                Bundle args = new Bundle();
                args.putString("questionId", plantDocModel.getQuestionId().toString());
                navigateToFragment(R.id.nav_controller_plant_doc_my_post_answer, (Activity) context, false, args);
            }
        });

        // show notification
        if (isInfoVisible)
            viewBinding.iconNotification.setVisibility(View.VISIBLE);
        else
            viewBinding.iconNotification.setVisibility(View.GONE);

        viewBinding.iconNotification.setImageResource(R.drawable.ic_info);

    }

    public void setText(CharSequence text) {
        this.text = text;
    }

    public CharSequence getText() {
        return text;
    }
}
