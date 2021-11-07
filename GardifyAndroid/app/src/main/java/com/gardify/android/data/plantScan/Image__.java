
package com.gardify.android.data.plantScan;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class Image__ {

    @SerializedName("link")
    @Expose
    private String link;
    @SerializedName("image")
    @Expose
    private Image___ image;

    public String getLink() {
        return link;
    }

    public void setLink(String link) {
        this.link = link;
    }

    public Image___ getImage() {
        return image;
    }

    public void setImage(Image___ image) {
        this.image = image;
    }

}
