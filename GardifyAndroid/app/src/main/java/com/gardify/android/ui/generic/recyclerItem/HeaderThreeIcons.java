package com.gardify.android.ui.generic.recyclerItem;

import android.content.Context;
import android.view.View;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemHeaderThreeIconBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class HeaderThreeIcons extends BindableItem<ItemHeaderThreeIconBinding> {

    private int header;
    private String headerString;
    private onIconClickListener onIconClickListener;
    private int imageOneId, imageTwoId, imageThreeId;
    private Context context;

    public HeaderThreeIcons(int header, Context context, int imageOneId, int imageTwoId, int imageThreeId, onIconClickListener onIconClickListener) {
        this.header = header;
        this.context=context;
        this.imageOneId=imageOneId;
        this.imageTwoId=imageTwoId;
        this.imageThreeId=imageThreeId;
        this.onIconClickListener = onIconClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    public HeaderThreeIcons(String headerString, Context context, int imageOneId, int imageTwoId, int imageThreeId, onIconClickListener onIconClickListener) {
        this.headerString = headerString;
        this.context=context;
        this.imageOneId=imageOneId;
        this.imageTwoId=imageTwoId;
        this.imageThreeId=imageThreeId;
        this.onIconClickListener = onIconClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.item_header_three_icon;
    }

    @Override
    public void bind(@NonNull ItemHeaderThreeIconBinding viewBinding, int position) {

        if (header != 0) {
            viewBinding.text.setText(header);
        } else {
            viewBinding.text.setText(headerString);
        }
        viewBinding.imageOne.setImageDrawable(ContextCompat.getDrawable(context, imageOneId));
        viewBinding.imageTwo.setImageDrawable(ContextCompat.getDrawable(context, imageTwoId));
        viewBinding.imageThree.setImageDrawable(ContextCompat.getDrawable(context, imageThreeId));

        viewBinding.imageOne.setOnClickListener(v -> {
            onIconClickListener.onClick(imageOneId, viewBinding.imageOne);
        });
        viewBinding.imageTwo.setOnClickListener(v -> {
            onIconClickListener.onClick(imageTwoId, viewBinding.imageTwo);
        });
        viewBinding.imageThree.setOnClickListener(v -> {
            onIconClickListener.onClick(imageThreeId, viewBinding.imageThree);
        });
    }

    public void setText(int header) {
        this.header = header;
    }

    public int getText() {
        return header;
    }

    public interface onIconClickListener {
        void onClick(int imageId, View view);
    }
}
