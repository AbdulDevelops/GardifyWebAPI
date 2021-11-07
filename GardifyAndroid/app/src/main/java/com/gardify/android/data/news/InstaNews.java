package com.gardify.android.data.news;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class InstaNews {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("data")
    @Expose
    private List<Datum> data = null;
    @SerializedName("paging")
    @Expose
    private Paging paging;

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }

    public List<Datum> getData() {
        return data;
    }

    public void setData(List<Datum> data) {
        this.data = data;
    }

    public Paging getPaging() {
        return paging;
    }

    public void setPaging(Paging paging) {
        this.paging = paging;
    }

    public class Datum {

        @SerializedName("$id")
        @Expose
        private String $id;
        @SerializedName("id")
        @Expose
        private String id;
        @SerializedName("caption")
        @Expose
        private String caption;
        @SerializedName("media_type")
        @Expose
        private String mediaType;
        @SerializedName("media_url")
        @Expose
        private String mediaUrl;
        @SerializedName("username")
        @Expose
        private String username;
        @SerializedName("timestamp")
        @Expose
        private String timestamp;
        @SerializedName("thumbnail_url")
        @Expose
        private Object thumbnailUrl;

        public String get$id() {
            return $id;
        }

        public void set$id(String $id) {
            this.$id = $id;
        }

        public String getId() {
            return id;
        }

        public void setId(String id) {
            this.id = id;
        }

        public String getCaption() {
            return caption;
        }

        public void setCaption(String caption) {
            this.caption = caption;
        }

        public String getMediaType() {
            return mediaType;
        }

        public void setMediaType(String mediaType) {
            this.mediaType = mediaType;
        }

        public String getMediaUrl() {
            return mediaUrl;
        }

        public void setMediaUrl(String mediaUrl) {
            this.mediaUrl = mediaUrl;
        }

        public String getUsername() {
            return username;
        }

        public void setUsername(String username) {
            this.username = username;
        }

        public String getTimestamp() {
            return timestamp;
        }

        public void setTimestamp(String timestamp) {
            this.timestamp = timestamp;
        }

        public Object getThumbnailUrl() {
            return thumbnailUrl;
        }

        public void setThumbnailUrl(Object thumbnailUrl) {
            this.thumbnailUrl = thumbnailUrl;
        }

    }

    public class Paging {

        @SerializedName("$id")
        @Expose
        private String $id;
        @SerializedName("next")
        @Expose
        private String next;
        @SerializedName("prev")
        @Expose
        private Object prev;

        public String get$id() {
            return $id;
        }

        public void set$id(String $id) {
            this.$id = $id;
        }

        public String getNext() {
            return next;
        }

        public void setNext(String next) {
            this.next = next;
        }

        public Object getPrev() {
            return prev;
        }

        public void setPrev(Object prev) {
            this.prev = prev;
        }

    }
}