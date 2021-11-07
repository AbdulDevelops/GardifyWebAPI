package com.gardify.android.ui.gardenGlossary.recyclerItem;

import android.content.Context;

import androidx.annotation.NonNull;

import com.gardify.android.data.gardenGlossary.Example;
import com.gardify.android.R;
import com.gardify.android.databinding.RecyclerViewGardenGlossaryFilterItemBinding;
import com.gardify.android.utils.APP_URL;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.utils.UiUtils.loadImageUsingGlide;

public class GardenGlossaryCardItem extends BindableItem<RecyclerViewGardenGlossaryFilterItemBinding> {

    private CharSequence text;
    private Context context;
    private Example example;
    boolean expand = false;

    public GardenGlossaryCardItem(Context context, Example example, CharSequence text) {
        this.context=context;
        this.example=example;
        this.text = text;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_garden_glossary_filter_item;
    }

    @Override
    public void bind(@NonNull RecyclerViewGardenGlossaryFilterItemBinding viewBinding, int position) {
        String imageURl = APP_URL.BASE_ROUTE_INTERN + example.getImages().get(0).getSrcAttr();
        loadImageUsingGlide(context,imageURl,viewBinding.imageViewGardenKnowledge);
        // Glide.with(context).load(imageURl).centerCrop().into(viewBinding.imageViewGardenKnowledgePlantFilterIcon);
        viewBinding.textViewGardenKnowledgeTitle.setText(example.getName());
        viewBinding.textViewGardenKnowledgeDescription.setText(example.getDescription());

        viewBinding.imageButtonGardenKnowledgeExpandIcon.setOnClickListener(v -> {
            if(!expand){
                viewBinding.imageButtonGardenKnowledgeExpandIcon.setImageResource(R.drawable.collapse);
                viewBinding.textViewGardenKnowledgeDescription.setMaxLines(50);
                expand = true;
            }else {
                viewBinding.imageButtonGardenKnowledgeExpandIcon.setImageResource(R.drawable.expand);
                viewBinding.textViewGardenKnowledgeDescription.setMaxLines(3);
                expand = false;
            }

        });

    }

    public void setText(CharSequence text) {
        this.text = text;
    }

    public CharSequence getText() {
        return text;
    }

}
