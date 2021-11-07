package com.gardify.android.ui.todosCalendar.recyclerItems;

import android.annotation.SuppressLint;
import android.content.Context;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;

import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.myGarden.UserGarden.UserGarden;
import com.gardify.android.R;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.utils.RequestType;
import com.gardify.android.databinding.ItemHeaderIconsTodoBinding;
import com.xwray.groupie.databinding.BindableItem;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class TodoHeaderIcons extends BindableItem<ItemHeaderIconsTodoBinding> {

    private int header;
    private String headerString;
    private onIconClickListener onIconClickListener;
    private int imageOneId, imageTwoId, imageThreeId;
    private Context context;
    private Boolean spinnerTouched = false;
    private int selectedSpinnerPos=0;
    private ItemHeaderIconsTodoBinding viewBinding;
    private List<UserGarden> userGardenList = null;

    public TodoHeaderIcons(int header, Context context, int imageOneId, int imageTwoId, int imageThreeId, onIconClickListener onIconClickListener) {
        this.header = header;
        this.context = context;
        this.imageOneId = imageOneId;
        this.imageTwoId = imageTwoId;
        this.imageThreeId = imageThreeId;
        this.onIconClickListener = onIconClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    public TodoHeaderIcons(String headerString, Context context, int selectedSpinnerPos, int imageOneId, int imageTwoId, int imageThreeId, onIconClickListener onIconClickListener) {
        this.headerString = headerString;
        this.context = context;
        this.imageOneId = imageOneId;
        this.selectedSpinnerPos = selectedSpinnerPos;
        this.imageTwoId = imageTwoId;
        this.imageThreeId = imageThreeId;
        this.onIconClickListener = onIconClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.item_header_icons_todo;
    }

    @SuppressLint("ClickableViewAccessibility")
    @Override
    public void bind(@NonNull ItemHeaderIconsTodoBinding binding, int position) {

        viewBinding = binding;
        //Spinner select listener -------- setSelection: by default nothing is selected

        binding.imageOne.setImageDrawable(ContextCompat.getDrawable(context, imageOneId));
        binding.imageTwo.setImageDrawable(ContextCompat.getDrawable(context, imageTwoId));
        binding.imageThree.setImageDrawable(ContextCompat.getDrawable(context, imageThreeId));

        binding.imageOne.setOnClickListener(v -> {
            onIconClickListener.onClick(imageOneId, binding, binding.imageOne, selectedSpinnerPos);
        });
        binding.imageTwo.setOnClickListener(v -> {
            onIconClickListener.onClick(imageTwoId, binding, binding.imageTwo, selectedSpinnerPos);
        });
        binding.imageThree.setOnClickListener(v -> {
            onIconClickListener.onClick(imageThreeId, binding, binding.imageThree, selectedSpinnerPos);
        });

        // set touch and click listener for spinner
        viewBinding.spinnerTodoFragment.setOnItemSelectedListener(onItemSelectedListener);
        viewBinding.spinnerTodoFragment.setOnTouchListener((v, event) -> {
            spinnerTouched = true;
            return false;
        });
        // populate Gartenbereich spinner
        if (userGardenList == null) {
            getUserGardenListFromApi();
        }
    }

    private void getUserGardenListFromApi() {
        String apiUrl = APP_URL.USER_LIST_API;
        RequestQueueSingleton.getInstance(context).typedRequest(apiUrl, this::onSuccessUserGarden, null, UserGarden[].class, new RequestData(RequestType.UserGarden));
    }

    private void onSuccessUserGarden(UserGarden[] model, RequestData data) {
        userGardenList = new ArrayList<>();
        userGardenList = new ArrayList<>(Arrays.asList(model));

        //position 0 spinner item
        UserGarden userGardenDefault = new UserGarden(0, "Gesamt");
        userGardenList.add(0, userGardenDefault);

        ArrayAdapter<UserGarden> countryArrayAdapter = new ArrayAdapter<>(context,  R.layout.custom_spinner_item, userGardenList); //selected item will look like a spinner set from XML
        viewBinding.spinnerTodoFragment.setAdapter(countryArrayAdapter);
        viewBinding.spinnerTodoFragment.setSelection(selectedSpinnerPos, false);
    }

    public AdapterView.OnItemSelectedListener onItemSelectedListener = new AdapterView.OnItemSelectedListener() {
        @Override
        public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
            // 0 is default position, which doesn't trigger onclick
            if (spinnerTouched) {
                spinnerTouched=false;
                selectedSpinnerPos = position;
                onIconClickListener.onClick(0, viewBinding, viewBinding.spinnerTodoFragment, selectedSpinnerPos);
            }
        }

        @Override
        public void onNothingSelected(AdapterView<?> parent) {
        }
    };

    public void setText(int header) {
        this.header = header;
    }

    public int getText() {
        return header;
    }

    public interface onIconClickListener {
        void onClick(int imageId, ItemHeaderIconsTodoBinding viewBinding, View view, int selectedSpinnerPos);
    }
}
