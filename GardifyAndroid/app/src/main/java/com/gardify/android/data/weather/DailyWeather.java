
package com.gardify.android.data.weather;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class DailyWeather {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("City")
    @Expose
    private String city;
    @SerializedName("Forecasts")
    @Expose
    private List<Forecast> forecasts = null;

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }

    public String getCity() {
        return city;
    }

    public void setCity(String city) {
        this.city = city;
    }

    public List<Forecast> getForecasts() {
        return forecasts;
    }

    public void setForecasts(List<Forecast> forecasts) {
        this.forecasts = forecasts;
    }

}
