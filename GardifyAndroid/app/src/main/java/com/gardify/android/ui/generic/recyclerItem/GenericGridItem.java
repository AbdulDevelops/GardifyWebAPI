package com.gardify.android.ui.generic.recyclerItem;

import android.content.Context;
import android.graphics.Bitmap;
import android.view.View;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.databinding.ItemGenericGridImageBinding;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.utils.UiUtils.loadImageUsingGlide;

public class GenericGridItem extends BindableItem<ItemGenericGridImageBinding> {

    private long id;
    private int colSpan;
    private String imageUrl;
    private Bitmap bitmap;
    private Context context;
    private ImageClickListener imageClickListener;

    public GenericGridItem(Builder builder) {
        super(builder.id);
        this.context = builder.context;
        this.colSpan = builder.colSpan;
        this.imageUrl = builder.imageUrl;
        this.bitmap = builder.bitmap;
        this.imageClickListener = builder.imageClickListener;
    }

    @Override
    public int getLayout() {
        return R.layout.item_generic_grid_image;
    }

    @Override
    public void bind(@NonNull final ItemGenericGridImageBinding binding, int position) {

        binding.image.setOnClickListener(v -> imageClickListener.onClick(binding, binding.image, position));
        if (bitmap == null) {
            loadImageUsingGlide(context, imageUrl, binding.image);
        } else {
            binding.image.setImageBitmap(bitmap);
        }
    }

    @Override
    public int getSpanSize(int spanCount, int position) {
        return spanCount / colSpan;
    }

    public static class Builder {
        private long id;
        private int colSpan;
        private Context context;
        private String imageUrl;
        private Bitmap bitmap;

        private ImageClickListener imageClickListener;

        public Builder(Context context) {
            this.context = context;
        }

        public Builder setId(long id) {
            this.id = id;
            return this;
        }

        public Builder setImageUrl(String imageUrl) {
            this.imageUrl = imageUrl;
            return this;
        }

        public Builder setBitmap(Bitmap bitmap) {
            this.bitmap = bitmap;
            return this;
        }

        public Builder setSpanCount(int colSpan) {
            this.colSpan = colSpan;
            return this;
        }

        public Builder setImageClickListener(ImageClickListener imageClickListener) {
            this.imageClickListener = imageClickListener;
            return this;
        }

        public GenericGridItem build() {
            return new GenericGridItem(this);
        }

    }

    public interface ImageClickListener {
        void onClick(ItemGenericGridImageBinding binding, View view, int position);
    }
}
