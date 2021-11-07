package com.gardify.android.ui.weather.recyclerItems;

import android.content.Context;

import androidx.annotation.NonNull;

import com.gardify.android.R;
import com.gardify.android.data.weather.Forecast;
import com.gardify.android.databinding.RecyclerViewWeatherDailyForecastBinding;
import com.gardify.android.utils.TimeUtils;
import com.xwray.groupie.databinding.BindableItem;

import static com.gardify.android.ui.weather.WeatherFragment.API_DATE_PATTERN;
import static com.gardify.android.utils.TimeUtils.checkDateToday;

/**
 * A card item with a fixed width so it can be used with a horizontal layout manager.
 */
public class DailyWeatherCarouselItem extends BindableItem<RecyclerViewWeatherDailyForecastBinding> {

    private Context context;
    private Forecast dailyForecast;

    public DailyWeatherCarouselItem(Context context, Forecast dailyForecast) {
        this.context=context;
        this.dailyForecast=dailyForecast;
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_view_weather_daily_forecast;
    }

    @Override
    public void bind(@NonNull RecyclerViewWeatherDailyForecastBinding binding, int position) {

        String day = checkDateToday(context, dailyForecast.getValidFrom(), API_DATE_PATTERN, "EEEE");
        String date = TimeUtils.dateToString(dailyForecast.getValidFrom(), API_DATE_PATTERN, "dd.MM");

        binding.textViewWeatherDailyForecastDay.setText(day);
        binding.textViewWeatherDailyForecastDate.setText(date);
        binding.textViewWeatherDailyForecastMax.setText(Math.round(dailyForecast.getMaxAirTemperatureInCelsius()) + " °C");
        binding.textViewWeatherDailyForecastMin.setText(Math.round(dailyForecast.getMinAirTemperatureInCelsius()) + " °C");
    }

}
