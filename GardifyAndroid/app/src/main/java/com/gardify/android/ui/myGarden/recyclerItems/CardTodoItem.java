package com.gardify.android.ui.myGarden.recyclerItems;

import android.content.Context;
import android.util.Log;
import android.view.View;
import android.widget.Toast;

import androidx.annotation.NonNull;

import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.R;
import com.gardify.android.data.misc.RequestData;
import com.gardify.android.data.myGarden.MyGarden;
import com.gardify.android.data.myGarden.UserPlant;
import com.gardify.android.databinding.RecyclerMyGardenTodoItemCardBinding;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.RequestQueueSingleton;
import com.xwray.groupie.databinding.BindableItem;

import org.json.JSONObject;

import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET;
import static com.gardify.android.ui.myGarden.MyGardenFragment.INSET_TYPE_KEY;

public class CardTodoItem extends BindableItem<RecyclerMyGardenTodoItemCardBinding> {

    private static final String TAG = "MyGarden CardTodoItem: ";
    private int bgColor;
    private Context context;
    private OnTodoCardClickListener onCardClickListener;
    private MyGarden myGarden;
    private RecyclerMyGardenTodoItemCardBinding _binding;
    private UserPlant.CyclicTodo cyclicTodo;
    private boolean checkboxUpdateFlag;
    public CardTodoItem(Context context, int bgColor, MyGarden myGarden, OnTodoCardClickListener onCardClickListener) {
        this.bgColor = bgColor;
        this.myGarden = myGarden;
        this.context = context;
        this.onCardClickListener = onCardClickListener;
        getExtras().put(INSET_TYPE_KEY, INSET);
    }

    @Override
    public int getLayout() {
        return R.layout.recycler_my_garden_todo_item_card;
    }

    String todoId;

    @Override
    public void bind(@NonNull RecyclerMyGardenTodoItemCardBinding viewBinding, int position) {
        _binding = viewBinding;
        cyclicTodo = myGarden.getUserPlant().getCyclicTodos().get(position);
        if (myGarden.getUserPlant().getTodos() != null && myGarden.getUserPlant().getTodos().get(position)!=null) {
            todoId = myGarden.getUserPlant().getTodos().get(position).getId().toString();
        }
        viewBinding.myGardenTodoText.setText(cyclicTodo.getTitle());
        viewBinding.checkboxMyGardenTodo.setChecked(cyclicTodo.getFinished());

        // click listener for marking todos
        _binding.checkboxMyGardenTodo.setOnClickListener(v -> {

            if (cyclicTodo.getFinished()) {
                checkboxUpdateFlag = false;
                String markTodoFalseApi = APP_URL.TODO_API + "markfinished/" + checkboxUpdateFlag + "/" + todoId + "/" + "0";
                RequestQueueSingleton.getInstance(context).objectRequest(markTodoFalseApi, Request.Method.PUT, this::onSuccessTodoCheckBoxUpdate, null, null);
            } else {
                checkboxUpdateFlag = true;
                String markTodoTrueApi = APP_URL.TODO_API + "markfinished/" + checkboxUpdateFlag + "/" + todoId + "/" + "0";
                RequestQueueSingleton.getInstance(context).objectRequest(markTodoTrueApi, Request.Method.PUT, this::onSuccessTodoCheckBoxUpdate, this::onErrorTodoCheckBoxUpdate, null);
            }
        });

        // click listener
        _binding.myGardenTodoExpandIcon.setOnClickListener(v -> onCardClickListener.onClick(_binding, _binding.myGardenTodoExpandIcon, cyclicTodo, cyclicTodo.getId()));
        _binding.myGardenTodoText.setOnClickListener(v -> onCardClickListener.onClick(_binding, _binding.myGardenTodoText, cyclicTodo, cyclicTodo.getId()));

    }

    private void onErrorTodoCheckBoxUpdate(VolleyError volleyError) {
        Toast.makeText(context, volleyError.getMessage(), Toast.LENGTH_SHORT).show();
    }

    private void onSuccessTodoCheckBoxUpdate(JSONObject jsonObject) {
        Log.d("CardTodoItem.java", "todo checkbox updated ");
        _binding.checkboxMyGardenTodo.setChecked(checkboxUpdateFlag);
    }

    private void onError(Exception e, RequestData requestData) {
        Log.d("MyGarden todo: ", "error cyclic id "+ cyclicTodo.getId());
    }

    public interface OnTodoCardClickListener {
        void onClick(RecyclerMyGardenTodoItemCardBinding binding, View view, UserPlant.CyclicTodo todoModel, int todoId);
    }
}
