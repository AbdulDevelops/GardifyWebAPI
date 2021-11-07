package com.gardify.android.data.community;

public class Community {

    private String header;
    private String body;
    private String link;
    private String footer;

    public Community(String header, String body, String link, String footer) {
        this.header = header;
        this.body = body;
        this.link = link;
        this.footer = footer;
    }

    public String getHeader() {
        return header;
    }

    public void setHeader(String header) {
        this.header = header;
    }

    public String getBody() {
        return body;
    }

    public void setBody(String body) {
        this.body = body;
    }

    public String getLink() {
        return link;
    }

    public void setLink(String link) {
        this.link = link;
    }

    public String getFooter() {
        return footer;
    }

    public void setFooter(String footer) {
        this.footer = footer;
    }
}
