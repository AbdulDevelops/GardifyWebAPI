package com.gardify.android.ui.plantScan.models;

import android.graphics.Bitmap;
import android.net.Uri;

public class ItemImage {
    private Bitmap imageViewBitmap;
    private Uri imageViewUri;
    private int id;

    public ItemImage(Bitmap imageViewBitmap, int id) {
        this.imageViewBitmap = imageViewBitmap;
        this.id = id;
    }
    public ItemImage(Uri imageViewUri, int id) {
        this.imageViewUri = imageViewUri;
        this.id = id;
    }

    public int getId() {
        return id;
    }

    public Bitmap getImageView() {
        return imageViewBitmap;
    }

    public Uri getImageViewUri(){
        return imageViewUri;
    }
}
