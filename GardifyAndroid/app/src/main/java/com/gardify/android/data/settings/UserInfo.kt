package com.gardify.android.data.settings

import com.google.gson.annotations.Expose
import com.google.gson.annotations.SerializedName

class UserInfo {
    @Expose
    @SerializedName("Country")
    var country: String? = null

    @Expose
    @SerializedName("Zip")
    var zip: String? = null

    @Expose
    @SerializedName("Street")
    var street: String? = null

    @Expose
    @SerializedName("City")
    var city: String? = null

    @Expose
    @SerializedName("UserName")
    var userName: String? = null

    @Expose
    @SerializedName("LastName")
    var lastName: String? = null

    @Expose
    @SerializedName("FirstName")
    var firstName: String? = null

    @Expose
    @SerializedName("HouseNr")
    var houseNr: String? = null

    @Expose
    @SerializedName("\$id")
    private var `$id`: String? = null

    fun `get$id`(): String? {
        return `$id`
    }

    fun `set$id`(`$id`: String?) {
        this.`$id` = `$id`
    }
}