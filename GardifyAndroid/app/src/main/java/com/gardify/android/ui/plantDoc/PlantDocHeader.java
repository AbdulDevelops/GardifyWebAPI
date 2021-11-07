package com.gardify.android.ui.plantDoc;

import android.app.Activity;
import android.content.Context;
import android.content.res.Resources;
import android.util.AttributeSet;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.SearchView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.Nullable;

import com.gardify.android.data.plantsDocModel.PlantDocViewModel;
import com.gardify.android.R;
import com.gardify.android.generic.RecycleViewGenericAdapter;

import static com.gardify.android.utils.UiUtils.navigateToFragment;


public class PlantDocHeader extends LinearLayout implements RecycleViewGenericAdapter.RecyclerViewRowHeader<PlantDocViewModel> {

    private Button buttonQuestion;
    private Button buttonPosts;
    private TextView searchTxtField;
    private ImageView imageViewExpandSearch;
    private SearchView searchView;
    private boolean expand = true;

    private RecycleViewGenericAdapter.OnTextChangeListener onTextChangeListener;

    Resources res;

    public PlantDocHeader(Context context) {
        super(context);
    }

    public PlantDocHeader(Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
    }

    public PlantDocHeader(Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }

    public void setOnTextChangeListener(RecycleViewGenericAdapter.OnTextChangeListener onTextChangeListener) {
        this.onTextChangeListener = onTextChangeListener;
    }

    @Override
    protected void onFinishInflate() {
        super.onFinishInflate();
        searchTxtField = findViewById(R.id.textView_plant_doc_header_search);
        imageViewExpandSearch = findViewById(R.id.imageView_plant_doc_header_expand_button);
        searchView = findViewById(R.id.searchView_plant_doc_header);
        buttonQuestion = findViewById(R.id.button_plant_doc_header_filter_my_posts);
        buttonPosts = findViewById(R.id.button_plant_doc_header_filter_question);
    }

    @Override
    public void showData(PlantDocViewModel item) {

        res = getContext().getResources();

        buttonQuestion.setText(res.getString(R.string.plantDoc_askQuestion));
        buttonQuestion.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                navigateToFragment(R.id.nav_controller_plant_doc_ask_question,(Activity) getContext(), false, null);

            }
        });


        buttonPosts.setText(res.getString(R.string.plantDoc_myPosts));
        buttonPosts.setOnClickListener(new View.OnClickListener(){
            @Override
            public void onClick(View v) {
                navigateToFragment(R.id.nav_controller_plant_doc_my_posts,(Activity) getContext(), false, null);
            }
        });

        searchTxtField.setText(res.getString(R.string.all_search));
        imageViewExpandSearch.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                if(expand){
                    searchView.setVisibility(VISIBLE);
                    imageViewExpandSearch.setImageResource(R.drawable.collapse);
                    expand = false;
                }else{
                    searchView.setVisibility(GONE);
                    imageViewExpandSearch.setImageResource(R.drawable.expand);
                    expand = true;
                }
            }
        });

        searchView.setOnQueryTextListener(new SearchView.OnQueryTextListener() {
            @Override
            public boolean onQueryTextSubmit(String query) {
                return false;
            }

            @Override
            public boolean onQueryTextChange(String newText) {
                Toast.makeText(getContext(), ""+newText, Toast.LENGTH_SHORT).show();
                onTextChangeListener.textSearch(newText);
                return true;
            }
        });
    }



}

