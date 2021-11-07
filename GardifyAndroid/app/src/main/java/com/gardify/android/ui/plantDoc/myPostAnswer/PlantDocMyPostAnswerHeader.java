package com.gardify.android.ui.plantDoc.myPostAnswer;

import android.content.Context;
import android.util.AttributeSet;
import android.view.View;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.annotation.Nullable;

import com.gardify.android.data.plantsDocModel.PlanDocViewModelAnswer;
import com.gardify.android.R;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.generic.RecycleViewGenericAdapter;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;

import static com.gardify.android.utils.UiUtils.loadImageUsingGlide;

public class PlantDocMyPostAnswerHeader extends LinearLayout implements RecycleViewGenericAdapter.RecyclerViewRowHeader<PlanDocViewModelAnswer> {

    private ImageView imageView;
    private TextView textViewDate;
    private TextView textViewHeader;
    private TextView textViewQuestion;
    private ImageView imageViewButton;
    private boolean expand = true;

    public PlantDocMyPostAnswerHeader(Context context) {
        super(context);
    }

    public PlantDocMyPostAnswerHeader(Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
    }

    public PlantDocMyPostAnswerHeader(Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }
    @Override
    public void setOnTextChangeListener(RecycleViewGenericAdapter.OnTextChangeListener onTextChangeListener) {
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();

        textViewHeader = findViewById(R.id.textView_plant_doc_my_post_answer_row_item_header);
        imageView = findViewById(R.id.imageView_plant_doc_my_post_answer_plant_item_plant_image);
        textViewDate = findViewById(R.id.textView_plant_doc_my_post_answer_row_item_date);
        textViewQuestion = findViewById(R.id.textView_plant_doc_my_post_answer_row_item_question1);
        imageViewButton = findViewById(R.id.imageView_plant_doc_my_post_answer_row_item_button);
    }

    @Override
    public void showData(PlanDocViewModelAnswer item) {
        String imageUrl = APP_URL.BASE_ROUTE + item.getPlantDocViewModel().getImages().get(0).getSrcAttr();
        loadImageUsingGlide(getContext(), imageUrl, imageView);

        SimpleDateFormat inFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS");
        SimpleDateFormat outFormat = new SimpleDateFormat("dd.MM.yyyy");
        Date d = null;
        try {
            d = inFormat.parse(item.getPlantDocViewModel().getPublishDate());
        } catch (ParseException e) {
            e.printStackTrace();
        }

        String formattedDate = outFormat.format(d);

        textViewDate.setText(formattedDate);

        textViewHeader.setText(item.getPlantDocViewModel().getThema());

        textViewQuestion.setText(item.getPlantDocViewModel().getQuestionText());
        textViewQuestion.setMaxLines(3);
        imageViewButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                if(expand){
                    imageViewButton.setImageResource(R.drawable.collapse);
                    textViewQuestion.setMaxLines(50);
                    expand = false;
                }else{
                    imageViewButton.setImageResource(R.drawable.expand);
                    textViewQuestion.setMaxLines(3);
                    expand = true;
                }
            }
        });

    }
}
