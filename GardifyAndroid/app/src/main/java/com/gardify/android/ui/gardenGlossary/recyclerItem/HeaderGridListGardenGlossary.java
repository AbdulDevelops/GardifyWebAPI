package com.gardify.android.ui.gardenGlossary.recyclerItem;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemHeaderGridListGardenGlossaryBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class HeaderGridListGardenGlossary extends BindableItem<ItemHeaderGridListGardenGlossaryBinding> {

    private int header;
    private onGridViewClickListener onGridViewClickListener;

    public HeaderGridListGardenGlossary(int header, onGridViewClickListener onGridViewClickListener) {
        this.header = header;
        this.onGridViewClickListener = onGridViewClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.item_header_grid_list_garden_glossary;
    }

    @Override
    public void bind(@NonNull ItemHeaderGridListGardenGlossaryBinding viewBinding, int position) {
        //viewBinding.getRoot().setBackgroundColor(colorRes);
        viewBinding.textGardenKnowledge.setText(header);
        viewBinding.imageViewGridGardenKnowledge.setOnClickListener(v -> {
            onGridViewClickListener.onClick(true);
        });
        viewBinding.imageViewListGardenKnowledge.setOnClickListener(v -> {
            onGridViewClickListener.onClick(false);
        });
    }

    public void setText(int header) {
        this.header = header;
    }

    public int getText() {
        return header;
    }

    public interface onGridViewClickListener {
        void onClick(boolean grid);
    }
}
