package com.gardify.android.ui.todosCalendar;

import android.app.DatePickerDialog;
import android.content.Intent;
import android.graphics.Bitmap;
import android.net.Uri;
import android.os.Bundle;
import android.text.InputType;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.android.volley.NetworkResponse;
import com.android.volley.Request;
import com.android.volley.VolleyError;
import com.gardify.android.R;
import com.gardify.android.generic.AutoSuggestAdapter;
import com.gardify.android.ui.generic.HeaderItemDecoration;
import com.gardify.android.ui.generic.recyclerItem.GenericGridItem;
import com.gardify.android.utils.APP_URL;
import com.gardify.android.utils.ApiUtils;
import com.gardify.android.utils.PreferencesUtility;
import com.gardify.android.utils.RequestImageUpload;
import com.gardify.android.utils.RequestQueueSingleton;
import com.gardify.android.viewModelData.todos.TodoCalendarViewModel;
import com.google.android.material.bottomsheet.BottomSheetDialogFragment;
import com.xwray.groupie.GroupAdapter;
import com.xwray.groupie.Section;

import org.apache.http.HttpEntity;
import org.apache.http.entity.ContentType;
import org.apache.http.entity.mime.HttpMultipartMode;
import org.apache.http.entity.mime.MultipartEntityBuilder;
import org.apache.http.entity.mime.content.StringBody;
import org.json.JSONObject;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.Locale;
import java.util.Map;

import static android.app.Activity.RESULT_OK;
import static com.gardify.android.ui.home.HomeFragment.RECYCLE_VIEW_DEFAULT_SPAN_COUNT;
import static com.gardify.android.utils.ImageUtils.convertUriToBitmap;
import static com.gardify.android.utils.ImageUtils.getImageBytes;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.showErrorDialogNetworkParsed;


public class AddTodoBottomSheetDialog extends BottomSheetDialogFragment implements View.OnClickListener {

    private static final int IMAGE_ALBUM = 1001;
    public static final int NON_CYCLIC_TODO = 0;
    public static final int CYCLIC_TODO_DAILY = 5;
    public static final int CYCLIC_TODO_WEEKLY = 1;
    public static final int CYCLIC_TODO_MONTHLY = 2;
    public static final int CYCLIC_TODO_YEARLY = 3;
    public static final int CYCLIC_TODO_2_YEAR = 4;
    public static final int CYCLIC_TODO_3_YEAR = 6;
    public static final int CYCLIC_TODO_4_YEAR = 7;
    public static final int CYCLIC_TODO_5_YEAR = 8;
    public static final int DEFAULT_REMINDER = 0;
    public static final int REMINDER_TYPE_PUSH = 2;
    public static final int REMINDER_TYPE_EMAIL = 3;
    public static final int REMINDER_TYPE_HIGHLIGHTED = 1;
    public static final int TODO_TYPE_CUSTOM = 12;
    private static final String TAG = "AddTodoBottomSheetDialog";

    private Button saveTodoBtn, cancelBtn;
    private EditText titleEdt, descriptionEdt, datePickerEdtFrom, datePickerEdtTo, notesEdt;
    private LinearLayout todoNotesLayout, repetitionLayout, todoFromLinearLayout, todoToLinearLayout,
            todoNotificationLayout, todoUploadSectionLayout, todoRecycleLinearLayout;
    private TextView headerTxt, textViewImageCount;
    private ImageView ImageUploadBtn, deleteImagesBtn;

    //Spinners
    private Spinner spinnerRepetitionCycle, spinnerNotification;
    private AutoSuggestAdapter adapterRepetitionCycle, adapterNotification;
    private ProgressBar progressBar;
    private int selectedCycle, selectedNotification;
    private TodoCalendarViewModel todos;
    private Bundle bundleArguments;
    // image upload
    private ArrayList<Bitmap> imageArrayList = new ArrayList<>();
    private GroupAdapter groupAdapter;
    private GridLayoutManager layoutManager;
    private RecyclerView recyclerView;

    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_todo_add_todo_entry, container, false);
        init(root);
        setupCycleSpinner();
        // setupNotificationSpinner();

        // edit todos view
        setupEditScreen();
        setupGroupAdapter();
        return root;

    }

    public void init(View root) {
        /* finding views block */
        headerTxt = root.findViewById(R.id.textView_add_todo_header);
        todoNotesLayout = root.findViewById(R.id.linearLayout_add_todo_notes);
        todoFromLinearLayout = root.findViewById(R.id.linearLayout_add_todo_date_from);
        todoToLinearLayout = root.findViewById(R.id.linearLayout_add_todo_date_to);
        todoUploadSectionLayout = root.findViewById(R.id.linearLayout_add_upload_section);
        todoRecycleLinearLayout = root.findViewById(R.id.linearLayout_add_todo_recyclerView_section);
        //todoNotificationLayout = root.findViewById(R.id.linearLayout_add_todo_notification);
        repetitionLayout = root.findViewById(R.id.linearLayout_add_todo_repetition);
        ImageUploadBtn = root.findViewById(R.id.image_view_add_custom_todo_upload_icon);
        textViewImageCount = root.findViewById(R.id.text_view_add_custom_todo_image_count);
        progressBar = root.findViewById(R.id.progressbar_addTodoEntry);
        recyclerView = root.findViewById(R.id.recycler_view_add_todo_grid_view);
        deleteImagesBtn = root.findViewById(R.id.image_view_add_custom_todo_delete_icon);
        saveTodoBtn = root.findViewById(R.id.button_add_todo_save_todo);
        cancelBtn = root.findViewById(R.id.button_add_todo_cancel);
        titleEdt = root.findViewById(R.id.editText_add_todo_title_name);
        descriptionEdt = root.findViewById(R.id.editText_add_todo_desc);
        notesEdt = root.findViewById(R.id.editText_add_todo_notes);

        datePickerEdtFrom = root.findViewById(R.id.editText_add_todo_from_date);
        datePickerEdtTo = root.findViewById(R.id.editText_add_todo_to_date);
        spinnerRepetitionCycle = root.findViewById(R.id.spinner_add_todo_cycle);
        // spinnerNotification = root.findViewById(R.id.spinner_add_todo_notification);

        datePickerEdtFrom.setInputType(InputType.TYPE_NULL);
        datePickerEdtTo.setInputType(InputType.TYPE_NULL);
        datePickerEdtTo.setEnabled(false);
        Calendar calendar = Calendar.getInstance();
        updateLabel(datePickerEdtFrom, calendar.getTime());
        updateLabel(datePickerEdtTo, calendar.getTime());

        ImageUploadBtn.setOnClickListener(this);
        deleteImagesBtn.setOnClickListener(this);
        datePickerEdtFrom.setOnClickListener(this);
        datePickerEdtTo.setOnClickListener(this);
        saveTodoBtn.setOnClickListener(this);
        cancelBtn.setOnClickListener(this);
    }

    private void setupEditScreen() {
        if (isEditView()) {
            String todoJsonString = bundleArguments.getString("TODO_CUSTOM");

            if (todoJsonString != null) {
                todos = ApiUtils.getGsonParser().fromJson(todoJsonString, TodoCalendarViewModel.class);
                // update UI
                headerTxt.setText("To-do bearbeiten");
                titleEdt.setText(todos.getTitle());
                descriptionEdt.setText(todos.getDescription());
                todoNotesLayout.setVisibility(View.VISIBLE);
                notesEdt.setText(todos.getNotes());
                //uneditable views
                //todoNotificationLayout.setVisibility(View.GONE);
                todoUploadSectionLayout.setVisibility(View.GONE);
                todoFromLinearLayout.setVisibility(View.GONE);
                todoToLinearLayout.setVisibility(View.GONE);
                repetitionLayout.setVisibility(View.GONE);
                todoRecycleLinearLayout.setVisibility(View.GONE);
            }
        }
    }

    private boolean isEditView() {
        bundleArguments = getArguments();
        return bundleArguments != null;
    }

    private void setupGroupAdapter() {
        groupAdapter = new GroupAdapter();
        groupAdapter.setSpanCount(RECYCLE_VIEW_DEFAULT_SPAN_COUNT);
        layoutManager = new GridLayoutManager(getContext(), groupAdapter.getSpanCount());
        layoutManager.setSpanSizeLookup(groupAdapter.getSpanSizeLookup());
        recyclerView.setLayoutManager(layoutManager);
        recyclerView.addItemDecoration(new HeaderItemDecoration(0, 0));
        recyclerView.setAdapter(groupAdapter);
    }

    private void setupCycleSpinner() {
        adapterRepetitionCycle = new AutoSuggestAdapter(getContext(), R.layout.custom_spinner_item);

        LinkedHashMap<String, Integer> content = new LinkedHashMap<>();

        content.put("Niemals", NON_CYCLIC_TODO);
        content.put("Täglich", CYCLIC_TODO_DAILY);
        content.put("Wöchentlich", CYCLIC_TODO_WEEKLY);
        content.put("Monatlich", CYCLIC_TODO_MONTHLY);
        content.put("Jährlich", CYCLIC_TODO_YEARLY);
        content.put("2-jährig", CYCLIC_TODO_2_YEAR);
        content.put("3-jährig", CYCLIC_TODO_3_YEAR);
        content.put("4-jährig", CYCLIC_TODO_4_YEAR);
        content.put("5-jährig", CYCLIC_TODO_5_YEAR);

        adapterRepetitionCycle.setData(content);
        spinnerRepetitionCycle.setAdapter(adapterRepetitionCycle);
        spinnerRepetitionCycle.setSelection(0);
        spinnerRepetitionCycle.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                String item = adapterRepetitionCycle.getItem(position);
                selectedCycle = content.get(item);

                // ToDate only enabled when selected cycle is not 0 or Niemals
                if (selectedCycle != 0) {
                    datePickerEdtTo.setEnabled(true);
                } else {
                    datePickerEdtTo.setEnabled(false);
                    datePickerEdtTo.setText("");
                }
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {
            }
        });
    }

    private void setupNotificationSpinner() {
        adapterNotification = new AutoSuggestAdapter(getContext(), R.layout.custom_spinner_item);

        LinkedHashMap<String, Integer> contentNotification = new LinkedHashMap<>();

        contentNotification.put("Kalendereintrag", DEFAULT_REMINDER);
        contentNotification.put("Kalendereintrag hervorheben", REMINDER_TYPE_HIGHLIGHTED);
        contentNotification.put("Pushnachricht", REMINDER_TYPE_PUSH);
        contentNotification.put("Emailnachricht", REMINDER_TYPE_EMAIL);

        adapterNotification.setData(contentNotification);
        spinnerNotification.setAdapter(adapterNotification);
        spinnerNotification.setSelection(0);
        spinnerNotification.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
                String item = adapterNotification.getItem(position);
                selectedNotification = contentNotification.get(item);
            }

            @Override
            public void onNothingSelected(AdapterView<?> parent) {
            }
        });
    }

    @Override
    public void onClick(View view) {

        switch (view.getId()) {

            case R.id.editText_add_todo_from_date:
                displayDatePicker(datePickerEdtFrom);
                break;

            case R.id.editText_add_todo_to_date:
                displayDatePicker(datePickerEdtTo);
                break;

            case R.id.image_view_add_custom_todo_upload_icon:
                pickImageFromGallery();
                break;

            case R.id.image_view_add_custom_todo_delete_icon:
                showUploadImageLayout();
                break;

            case R.id.button_add_todo_cancel:
                dismiss();
                break;

            case R.id.button_add_todo_save_todo:

                Map<String, String> params = new HashMap<>();
                JSONObject jsonObject;

                if (isEditView()) {
                    // edit notes
                    if (titleEdt.getText().toString().length() < 1) {
                        titleEdt.setError(getContext().getResources().getString(R.string.all_required));
                        break;
                    } else {
                        // updated params
                        params.put("Title", titleEdt.getText().toString());
                        params.put("Description", descriptionEdt.getText().toString());
                        params.put("Notes", notesEdt.getText().toString());
                        // other params
                        if (todos.getReferenceType() != TODO_TYPE_CUSTOM) {
                            params.put("CyclicId", String.valueOf(todos.getCyclicId()));
                        }
                        params.put("DateEnd", todos.getDateEnd());
                        params.put("DateStart", todos.getDateStart());
                        params.put("Id", String.valueOf(todos.getId()));
                        params.put("Finished", String.valueOf(todos.isFinished()));
                        params.put("Ignored", String.valueOf(todos.isDeleted()));
                        params.put("UserId", PreferencesUtility.getUser(getContext()).getUserId());
                        params.put("Deleted", String.valueOf(todos.isIgnored()));
                        params.put("ReferenceType", String.valueOf(todos.getReferenceType()));
                        jsonObject = new JSONObject(params);

                        String editTodoUrl = APP_URL.TODO_API + todos.getId() + "/uploadTodo";
                        RequestQueueSingleton.getInstance(getContext()).stringRequest(editTodoUrl, Request.Method.PUT, this::onSuccessSaveTodo, this::onError, jsonObject);
                        progressBar.setVisibility(View.VISIBLE);
                    }
                } else { // add new note

                    if (titleEdt.getText().toString().length() < 1) {
                        titleEdt.setError(getContext().getResources().getString(R.string.all_required));
                        break;
                    } else if (descriptionEdt.getText().toString().length() < 1) {
                        descriptionEdt.setError(getContext().getResources().getString(R.string.all_required));
                        break;
                    } else if (datePickerEdtFrom.getText().toString().length() < 1) {
                        datePickerEdtFrom.setError(getContext().getResources().getString(R.string.all_required));
                        break;
                    } else if (datePickerEdtTo.getText().toString().length() < 1 && datePickerEdtTo.isEnabled()) {
                        datePickerEdtTo.setError(getContext().getResources().getString(R.string.all_required));
                        break;
                    }

                    params.put("Title", titleEdt.getText().toString());
                    params.put("Description", descriptionEdt.getText().toString());
                    params.put("Notes", notesEdt.getText().toString());
                    params.put("DateStart", datePickerEdtFrom.getText().toString());
                    params.put("DateEnd", datePickerEdtTo.getText().toString());
                    params.put("Notification", String.valueOf(selectedNotification));
                    params.put("Cycle", String.valueOf(selectedCycle));
                    params.put("ReferenceType", String.valueOf(TODO_TYPE_CUSTOM));
                    params.put("ReferenceId", "0");
                    params.put("Precision", "0");
                    params.put("Images", null);


                    String addTodoUrl = APP_URL.TODO_API;
                    jsonObject = new JSONObject(params);

                    RequestQueueSingleton.getInstance(getContext()).stringRequest(addTodoUrl, Request.Method.POST, this::onSuccessSaveTodo, this::onError, jsonObject);
                    progressBar.setVisibility(View.VISIBLE);

                }
                break;
        }
    }

    private void showUploadImageLayout() {
        imageArrayList.clear();
        groupAdapter.clear();
        todoUploadSectionLayout.setVisibility(View.VISIBLE);
        todoRecycleLinearLayout.setVisibility(View.GONE);
        textViewImageCount.setText("0 Bild(er) ausgewählt");
    }

    private void onSuccessSaveTodo(String todoId) {

        if (hasImage() && getActivity() != null) {
            initiateImageUpload(todoId);
        } else {
            progressBar.setVisibility(View.GONE);
            Toast.makeText(getContext(), "Eintrag hinzugefügt", Toast.LENGTH_SHORT).show();
            dismiss();
            navigateToFragment(R.id.nav_controller_todo, getActivity(), true, null);
        }
    }

    private boolean hasImage() {
        return imageArrayList.size() > 0;
    }

    private void onError(VolleyError error) {
        showErrorDialogNetworkParsed(getContext(), error);
        progressBar.setVisibility(View.GONE);
    }

    private void initiateImageUpload(String todoId) {
        Iterator<Bitmap> iterator = imageArrayList.iterator();
        while (iterator.hasNext()) {
            Bitmap bitmap = iterator.next();
            uploadImage(todoId, bitmap);
            if (!iterator.hasNext()) {
                //last item
                if (isVisible()) {
                    progressBar.setVisibility(View.GONE);
                    Toast.makeText(getContext(), "Eintrag hinzugefügt", Toast.LENGTH_SHORT).show();
                    dismiss();
                    navigateToFragment(R.id.nav_controller_todo, getActivity(), true, null);
                }
            }
        }
    }

    private void uploadImage(String todoId, Bitmap bitmapImage) {

        Map<String, String> params = new HashMap<>();

        HttpEntity httpEntity;
        MultipartEntityBuilder builder = MultipartEntityBuilder.create();
        builder.setMode(HttpMultipartMode.BROWSER_COMPATIBLE);

        params.put("Id", todoId);
        params.put("ImageTitle", "Image_" + new Date().getTime() + ".jpg");

        // Add binary body
        ContentType contentType = ContentType.create("image/jpeg");
        String fileName = "todo_image_" + new Date().getTime() + ".jpg";
        if (bitmapImage != null) {
            builder.addBinaryBody("ImageFile", getImageBytes(bitmapImage), contentType, fileName);
        }
        // adding params
        for (String key : params.keySet()) {
            builder.addPart(key, new StringBody(params.get(key), ContentType.MULTIPART_FORM_DATA.withCharset("UTF-8")));
        }

        httpEntity = builder.build();
        String addTodoUrl = APP_URL.TODO_API + "upload";

        Request request  = new RequestImageUpload(getContext())
                .imageRequest(addTodoUrl, this::onSuccessImageUpload, this::onErrorImageUpload, httpEntity);

        RequestQueueSingleton.getInstance(getContext()).addToRequestQueue(request);

    }

    private void onErrorImageUpload(VolleyError volleyError) {
        Toast.makeText(getContext(), volleyError.toString(), Toast.LENGTH_SHORT).show();
    }

    private void onSuccessImageUpload(NetworkResponse networkResponse) {
        String resultResponse = new String(networkResponse.data);
        Log.d(TAG, "image successfully uploaded");
    }

    private void pickImageFromGallery() {
        Intent intent = new Intent();
        intent.setType("image/*");
        intent.putExtra(Intent.EXTRA_ALLOW_MULTIPLE, true);
        intent.setAction(Intent.ACTION_GET_CONTENT);
        startActivityForResult(Intent.createChooser(intent, "Select Picture"), IMAGE_ALBUM);
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);// FOR CHOOSING MULTIPLE IMAGES
        try {
            if (requestCode == IMAGE_ALBUM && resultCode == RESULT_OK && null != data) {
                addImagesToArrayList(data);
            }
        } catch (Exception e) {
            Toast.makeText(getContext(), R.string.toDoCalendar_errorSomethingWentWrong, Toast.LENGTH_SHORT).show();
        }
        textViewImageCount.setText(imageArrayList.size() + " Bild(er) ausgewählt");
        showImageGridView();
    }

    private void addImagesToArrayList(Intent data) {
        if (data.getClipData() != null) {
            int count = data.getClipData().getItemCount(); //evaluate the count before the for loop --- otherwise, the count is evaluated every loop.
            for (int i = 0; i < count; i++) {
                Uri imageUri = data.getClipData().getItemAt(i).getUri();
                Bitmap bitmap = convertUriToBitmap(getContext(), imageUri);
                imageArrayList.add(bitmap);
                Log.e(TAG, "onActivityResult: " + imageUri);
            }
        } else if (data.getData() != null) {
            Uri imageUri = data.getData();
            Bitmap bitmap = convertUriToBitmap(getContext(), imageUri);
            imageArrayList.add(bitmap);
            Log.e(TAG, "onActivityResult: " + imageUri);
        }
    }

    GenericGridItem gridItem;
    private void showImageGridView() {
        todoRecycleLinearLayout.setVisibility(View.VISIBLE);
        todoUploadSectionLayout.setVisibility(View.GONE);
        Section imageSection = new Section();
        for (int i = 0; i < imageArrayList.size(); i++) {

            gridItem = new GenericGridItem.Builder(getContext())
                    .setId(i)
                    .setBitmap(imageArrayList.get(i))
                    .setSpanCount(3)
                    .build();

            imageSection.add(gridItem);
        }
        groupAdapter.add(imageSection);
    }

    private void displayDatePicker(EditText editText) {
        final Calendar myCalendar = Calendar.getInstance();

        DatePickerDialog.OnDateSetListener date = (view, year, monthOfYear, dayOfMonth) -> {
            myCalendar.set(Calendar.YEAR, year);
            myCalendar.set(Calendar.MONTH, monthOfYear);
            myCalendar.set(Calendar.DAY_OF_MONTH, dayOfMonth);
            updateLabel(editText, myCalendar.getTime());
        };

        new DatePickerDialog(getContext(), date, myCalendar
                .get(Calendar.YEAR), myCalendar.get(Calendar.MONTH),
                myCalendar.get(Calendar.DAY_OF_MONTH)).show();
    }

    private void updateLabel(EditText editText, Date time) {
        String myFormat = "yyyy-MM-dd"; //In which you need put here
        SimpleDateFormat sdf = new SimpleDateFormat(myFormat, Locale.getDefault());
        editText.setText(sdf.format(time));
    }

    /**
     * Check if user entered info (either by authenticating or by entering the data manually)
     * exists. If it doesn't, redirect to LoginFragment.
     */
    @Override
    public void onResume() {
        super.onResume();

        if (!PreferencesUtility.getLoggedIn(getActivity())) {
            navigateToFragment(R.id.nav_controller_login, getActivity(), true, null);
        }
    }

    @Override
    public int getTheme() {
        return R.style.BaseBottomSheetDialog;
    }
}