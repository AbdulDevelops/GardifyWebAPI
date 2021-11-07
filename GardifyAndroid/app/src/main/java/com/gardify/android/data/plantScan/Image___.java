
package com.gardify.android.data.plantScan;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class Image___ {

    @SerializedName("thumbnailLink")
    @Expose
    private String thumbnailLink;
    @SerializedName("contextLink")
    @Expose
    private String contextLink;

    public String getThumbnailLink() {
        return thumbnailLink;
    }

    public void setThumbnailLink(String thumbnailLink) {
        this.thumbnailLink = thumbnailLink;
    }

    public String getContextLink() {
        return contextLink;
    }

    public void setContextLink(String contextLink) {
        this.contextLink = contextLink;
    }

}
