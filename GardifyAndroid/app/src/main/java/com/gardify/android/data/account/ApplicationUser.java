package com.gardify.android.data.account;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class ApplicationUser {

    @SerializedName("Token")
    @Expose
    private String Token;
    @SerializedName("ExpiresUtc")
    @Expose
    private String ExpiresUtc;
    @SerializedName("UserId")
    @Expose
    private String UserId;
    @SerializedName("Email")
    @Expose
    private String Email;
    @SerializedName("Name")
    @Expose
    private String Name;
    @SerializedName("Admin")
    @Expose
    private String Admin;

    public String getToken() {
        return Token;
    }

    public void setToken(String token) {
        Token = token;
    }

    public String getExpiresUtc() {
        return ExpiresUtc;
    }

    public void setExpiresUtc(String expiresUtc) {
        ExpiresUtc = expiresUtc;
    }

    public String getUserId() {
        return UserId;
    }

    public void setUserId(String userId) {
        UserId = userId;
    }

    public String getEmail() {
        return Email;
    }

    public void setEmail(String email) {
        Email = email;
    }

    public String getName() {
        return Name;
    }

    public void setName(String name) {
        Name = name;
    }

    public String getAdmin() {
        return Admin;
    }

    public void setAdmin(String admin) {
        Admin = admin;
    }
}