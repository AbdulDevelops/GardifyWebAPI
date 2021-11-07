package com.gardify.android.ui.weather.recyclerItems;

import android.view.View;

import androidx.annotation.NonNull;
import com.gardify.android.R;
import com.gardify.android.data.weather.Forecast;
import com.gardify.android.databinding.RecyclerItemWeatherForecastHourlyBinding;
import com.gardify.android.ui.weather.WeatherFragment;
import com.gardify.android.utils.TimeUtils;
import com.xwray.groupie.databinding.BindableItem;

import java.util.List;

import static com.gardify.android.ui.weather.WeatherFragment.API_DATE_PATTERN;
import static com.gardify.android.ui.weather.WeatherFragment.setWindOrigin;
import static com.gardify.android.ui.weather.WeatherFragment.setWindStrength;

/**
 * A card item with a fixed width so it can be used with a horizontal layout manager.
 */
public class HourlyWeatherCardItem extends BindableItem<RecyclerItemWeatherForecastHourlyBinding> {

    private Forecast hourlyForecast;
    private List<Forecast> forecastsPrecipitation; // contains precipitation

    public HourlyWeatherCardItem(Forecast hourlyForecast, List<Forecast> forecastsPrecipitation) {
        this.hourlyForecast= hourlyForecast;
        this.forecastsPrecipitation = forecastsPrecipitation;
    }


    @Override
    public int getLayout() {
        return R.layout.recycler_item_weather_forecast_hourly;
    }

    @Override
    public void bind(@NonNull RecyclerItemWeatherForecastHourlyBinding binding, int position) {

        //today weather detail

        String windStrength = setWindStrength(hourlyForecast.getWindSpeedInBeaufort());
        String windOrigin = setWindOrigin(hourlyForecast.getWindDirectionInDegree());
        String windSpeed = hourlyForecast.getWindSpeedInKilometerPerHour() + "km/h";

        String hourTime = TimeUtils.dateToString(hourlyForecast.getValidFrom(), API_DATE_PATTERN, "HH : mm");
        binding.textViewWeatherForecastHourlyTime.setText(hourTime);

        binding.linearLayoutTemperature.setVisibility(View.VISIBLE);
        setWeatherClarityIcon(binding);
        binding.textViewTemperature.setText(String.format(" %s °C", Math.round(hourlyForecast.getAirTemperatureInCelsius())));
        String precipitationPercent = String.valueOf(hourlyForecast.getPrecipitationAmountInMillimeter()!=null ? hourlyForecast.getPrecipitationAmountInMillimeter() : "0,0");
        binding.textViewWeatherPrecipitation.setText(hourlyForecast.getPrecipitationProbabilityInPercent()+"% /"+ precipitationPercent +"l/m\u00B2");
        binding.textViewWeatherWindDirectionSpeed.setText(String.format("%s aus %s (%s)", windStrength, windOrigin, windSpeed));
        binding.textViewWeatherAirPressure.setText(String.format("Luftdruck: %s hPa", hourlyForecast.getAirPressureAtSeaLevelInHectoPascal()));
        binding.linearLayoutHumidity.setVisibility(View.GONE);

    }

    private void setWeatherClarityIcon(@NonNull com.gardify.android.databinding.RecyclerItemWeatherForecastHourlyBinding binding) {
        new WeatherFragment().getWeatherClarity(hourlyForecast, hourlyForecast.getPrecipitationAmountInMillimeter(),
                forecastsPrecipitation, binding.imageViewWeatherClarity);
    }

}
