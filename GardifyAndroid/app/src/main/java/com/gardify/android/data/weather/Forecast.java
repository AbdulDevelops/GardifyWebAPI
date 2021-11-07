
package com.gardify.android.data.weather;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class Forecast {

    @SerializedName("$id")
    @Expose
    private String $id;
    @SerializedName("LocatedAt")
    @Expose
    private List<Double> locatedAt = null;
    @SerializedName("ValidFrom")
    @Expose
    private String validFrom;
    @SerializedName("ValidUntil")
    @Expose
    private String validUntil;
    @SerializedName("AirTemperatureInCelsius")
    @Expose
    private Double airTemperatureInCelsius;
    @SerializedName("AirPressureAtSeaLevelInHectoPascal")
    @Expose
    private Double airPressureAtSeaLevelInHectoPascal;
    @SerializedName("WindSpeedInKilometerPerHour")
    @Expose
    private Double windSpeedInKilometerPerHour;
    @SerializedName("WindDirectionInDegree")
    @Expose
    private Integer windDirectionInDegree;
    @SerializedName("PrecipitationProbabilityInPercent")
    @Expose
    private Integer precipitationProbabilityInPercent;
    @SerializedName("WeatherCode")
    @Expose
    private Integer weatherCode;
    @SerializedName("ValidPeriod")
    @Expose
    private String validPeriod;
    @SerializedName("PrecipitationAmountInMillimeter")
    @Expose
    private Double precipitationAmountInMillimeter;
    @SerializedName("WindSpeedInBeaufort")
    @Expose
    private Integer windSpeedInBeaufort;
    @SerializedName("RelativeHumidityInPercent")
    @Expose
    private Integer relativeHumidityInPercent;
    @SerializedName("CloudCoverLowerThan2000MeterInOcta")
    @Expose
    private Double cloudCoverLowerThan2000MeterInOcta;

    @SerializedName("MinAirTemperatureInCelsius")
    @Expose
    private Double minAirTemperatureInCelsius;
    @SerializedName("MaxAirTemperatureInCelsius")
    @Expose
    private Double maxAirTemperatureInCelsius;
    @SerializedName("SunshineDurationInMinutes")
    @Expose
    private Double sunshineDurationInMinutes;

    public String get$id() {
        return $id;
    }

    public void set$id(String $id) {
        this.$id = $id;
    }

    public List<Double> getLocatedAt() {
        return locatedAt;
    }

    public void setLocatedAt(List<Double> locatedAt) {
        this.locatedAt = locatedAt;
    }

    public String getValidFrom() {
        return validFrom;
    }

    public void setValidFrom(String validFrom) {
        this.validFrom = validFrom;
    }

    public String getValidUntil() {
        return validUntil;
    }

    public void setValidUntil(String validUntil) {
        this.validUntil = validUntil;
    }

    public Double getAirTemperatureInCelsius() {
        return airTemperatureInCelsius;
    }

    public void setAirTemperatureInCelsius(Double airTemperatureInCelsius) {
        this.airTemperatureInCelsius = airTemperatureInCelsius;
    }

    public Double getAirPressureAtSeaLevelInHectoPascal() {
        return airPressureAtSeaLevelInHectoPascal;
    }

    public void setAirPressureAtSeaLevelInHectoPascal(Double airPressureAtSeaLevelInHectoPascal) {
        this.airPressureAtSeaLevelInHectoPascal = airPressureAtSeaLevelInHectoPascal;
    }

    public Double getWindSpeedInKilometerPerHour() {
        return windSpeedInKilometerPerHour;
    }

    public void setWindSpeedInKilometerPerHour(Double windSpeedInKilometerPerHour) {
        this.windSpeedInKilometerPerHour = windSpeedInKilometerPerHour;
    }

    public Integer getWindDirectionInDegree() {
        return windDirectionInDegree;
    }

    public void setWindDirectionInDegree(Integer windDirectionInDegree) {
        this.windDirectionInDegree = windDirectionInDegree;
    }

    public Integer getPrecipitationProbabilityInPercent() {
        return precipitationProbabilityInPercent;
    }

    public void setPrecipitationProbabilityInPercent(Integer precipitationProbabilityInPercent) {
        this.precipitationProbabilityInPercent = precipitationProbabilityInPercent;
    }

    public Integer getWeatherCode() {
        return weatherCode;
    }

    public void setWeatherCode(Integer weatherCode) {
        this.weatherCode = weatherCode;
    }

    public String getValidPeriod() {
        return validPeriod;
    }

    public void setValidPeriod(String validPeriod) {
        this.validPeriod = validPeriod;
    }

    public Double getPrecipitationAmountInMillimeter() {
        return precipitationAmountInMillimeter;
    }

    public void setPrecipitationAmountInMillimeter(Double precipitationAmountInMillimeter) {
        this.precipitationAmountInMillimeter = precipitationAmountInMillimeter;
    }

    public Integer getWindSpeedInBeaufort() {
        return windSpeedInBeaufort;
    }

    public void setWindSpeedInBeaufort(Integer windSpeedInBeaufort) {
        this.windSpeedInBeaufort = windSpeedInBeaufort;
    }

    public Integer getRelativeHumidityInPercent() {
        return relativeHumidityInPercent;
    }

    public void setRelativeHumidityInPercent(Integer relativeHumidityInPercent) {
        this.relativeHumidityInPercent = relativeHumidityInPercent;
    }

    public Double getCloudCoverLowerThan2000MeterInOcta() {
        return cloudCoverLowerThan2000MeterInOcta;
    }

    public void setCloudCoverLowerThan2000MeterInOcta(Double cloudCoverLowerThan2000MeterInOcta) {
        this.cloudCoverLowerThan2000MeterInOcta = cloudCoverLowerThan2000MeterInOcta;
    }

    public Double getMinAirTemperatureInCelsius() {
        return minAirTemperatureInCelsius;
    }

    public void setMinAirTemperatureInCelsius(Double minAirTemperatureInCelsius) {
        this.minAirTemperatureInCelsius = minAirTemperatureInCelsius;
    }

    public Double getMaxAirTemperatureInCelsius() {
        return maxAirTemperatureInCelsius;
    }

    public void setMaxAirTemperatureInCelsius(Double maxAirTemperatureInCelsius) {
        this.maxAirTemperatureInCelsius = maxAirTemperatureInCelsius;
    }

    public Double getSunshineDurationInMinutes() {
        return sunshineDurationInMinutes;
    }

    public void setSunshineDurationInMinutes(Double sunshineDurationInMinutes) {
        this.sunshineDurationInMinutes = sunshineDurationInMinutes;
    }
}
