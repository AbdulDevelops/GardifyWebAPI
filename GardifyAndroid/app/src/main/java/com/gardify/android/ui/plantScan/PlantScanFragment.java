package com.gardify.android.ui.plantScan;

import android.Manifest;
import android.content.Context;
import android.content.ContextWrapper;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.drawable.BitmapDrawable;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ExpandableListView;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.cardview.widget.CardView;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.RecyclerView;

import com.gardify.android.R;
import com.gardify.android.generic.GenericDialog;
import com.gardify.android.generic.SaveImageToGallery;
import com.gardify.android.ui.plantScan.adapters.ItemClickListener;
import com.gardify.android.ui.plantScan.adapters.Section;
import com.gardify.android.ui.plantScan.adapters.SectionedExpandableLayoutHelper;
import com.gardify.android.ui.plantScan.models.ItemImage;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import static android.app.Activity.RESULT_OK;
import static com.gardify.android.utils.ImageUtils.convertUriToBitmap;
import static com.gardify.android.utils.UiUtils.navigateToFragment;
import static com.gardify.android.utils.UiUtils.setupToolbar;

public class PlantScanFragment extends Fragment implements ItemClickListener {

    public static final int EXTERNAL_STORAGE_WRITE_REQUEST_CODE = 1011;
    private static final int IMAGE_CAMERA = 1000;
    private static final int IMAGE_ALBUM = 1001;
    private static int count = 0;

    private ProgressBar progressBar;
    private ImageView imageView;
    private CardView cardViewCamera;
    private Button btnAlbum;
    private CardView cardViewAlbum;
    private CardView cardViewA;
    private TextView textViewNotFound;
    private Button buttonAskExpert;
    private LinearLayout linearLayoutPlanScan;

    private Uri imageUri;
    private Bitmap cameraBitmap;
    private byte[] image;

    private ExpandableListView listView;
    private List<String> listDataHeader;
    private HashMap<String, List<String>> listHash;

    private ArrayList<ItemImage> imageArrayList;
    private SectionedExpandableLayoutHelper sectionedExpandableLayoutHelper1;
    private SectionedExpandableLayoutHelper sectionedExpandableLayoutHelper2;

    private ArrayList<File> fileList = new ArrayList<File>();

    private boolean successfulScan = true;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        Bundle scanResultBundle = this.getArguments();
        if (scanResultBundle != null) {
            successfulScan = scanResultBundle.getBoolean("success", true);
        }

        // Inflate the layout for this fragment
        View root = inflater.inflate(R.layout.fragment_plant_scanner, container, false);

        setupToolbar(getActivity(), "PFLANZEN SCAN", R.drawable.gardify_app_icon_pflanzen_erkennen, R.color.toolbar_plantScan_setupToolbar, true);

        linearLayoutPlanScan = root.findViewById(R.id.linearLayout_plant_scan);
        progressBar = root.findViewById(R.id.progressBar_plant_scanner);

        View rootInToLinearLayout = getLayoutInflater().inflate(R.layout.view_plant_scan, linearLayoutPlanScan, false);

        RecyclerView recyclerViewTipps = rootInToLinearLayout.findViewById(R.id.recycler_view_tipps);
        RecyclerView mRecyclerViewGridView = rootInToLinearLayout.findViewById(R.id.recycler_view_grid_view);

        imageArrayList = new ArrayList<>();
        sectionedExpandableLayoutHelper1 = new SectionedExpandableLayoutHelper(getContext(),
                mRecyclerViewGridView, this, 3, true);

        sectionedExpandableLayoutHelper1.addSection1("Letzte Scan", imageArrayList, null);
        sectionedExpandableLayoutHelper1.notifyDataSetChanged();

        ArrayList<String> tippsString = new ArrayList<>();
        tippsString.add("Pflanzenfoto zur Pflanzenbestimmung hochladen: So gelingt es!");
        tippsString.add("• Es kann nur ein Bild pro Scan upgeloaded und verarbeitet werden");
        tippsString.add("• Bitte keine Fotos über 4MB Dateigröße hochladen. Größere Bilder bitte beschneiden oder kleinere Auflösung verwenden");
        tippsString.add("• Bitte haben Sie etwas Geduld, bis die Ergebnisse sichtbar werden");
        tippsString.add("• Nach Möglichkeit Blüten oder Knospen mitfotografieren");
        tippsString.add("• Immer bei ausreichend Licht fotografieren, Gegenlicht vermeiden");
        tippsString.add("• Immer nur eine Pflanze im Bild haben, ggf. näher rangehen");
        tippsString.add("• Wenn ihr kein brauchbares Ergebnis bekommt, schickt uns das Bild per Mail an team@gardify.de");

        sectionedExpandableLayoutHelper2 = new SectionedExpandableLayoutHelper(getContext(),
                recyclerViewTipps, this, 1, false);

        sectionedExpandableLayoutHelper2.addSection1("Tipps", null, tippsString);
        sectionedExpandableLayoutHelper2.notifyDataSetChanged();
        Section tipps = sectionedExpandableLayoutHelper2.getSection("Tipps");
        sectionedExpandableLayoutHelper2.onSectionStateChanged(tipps, false);
        progressBar.setVisibility(View.GONE);

        imageView = rootInToLinearLayout.findViewById(R.id.imageView_view_plant_scan);
        cardViewCamera = rootInToLinearLayout.findViewById(R.id.cardView_view_plant_scan_camera);
        cardViewAlbum = rootInToLinearLayout.findViewById(R.id.cardView_view_plant_scan_album);

        buttonAskExpert = rootInToLinearLayout.findViewById(R.id.button_plant_scan_askExpert);
        textViewNotFound = rootInToLinearLayout.findViewById(R.id.textView_plant_scan_notFound);

        cardViewCamera.setOnClickListener(v -> {
            Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
            startActivityForResult(intent, IMAGE_CAMERA);
        });

        cardViewAlbum.setOnClickListener(v -> {
            Intent intent = new Intent(Intent.ACTION_PICK, MediaStore.Images.Media.EXTERNAL_CONTENT_URI);
            startActivityForResult(intent, IMAGE_ALBUM);
        });

        loadImageFromStorage();

        initData();

        linearLayoutPlanScan.addView(rootInToLinearLayout);

        if (!successfulScan) {
            buttonAskExpert.setVisibility(View.VISIBLE);
            textViewNotFound.setVisibility(View.VISIBLE);
            buttonAskExpert.setOnClickListener(onButtonClickListener);
        } else {
            buttonAskExpert.setVisibility(View.GONE);
            textViewNotFound.setVisibility(View.GONE);
        }

        return root;
    }

    private void GotoResultFragment() {
        BitmapDrawable drawable1 = (BitmapDrawable) imageView.getDrawable();
        Bitmap bitmap = drawable1.getBitmap();
        ByteArrayOutputStream stream = new ByteArrayOutputStream();
        bitmap.compress(Bitmap.CompressFormat.JPEG, 100, stream);
        final byte[] bitmapData = stream.toByteArray();

        Bundle bundle = new Bundle();
        bundle.putByteArray("IMAGE", bitmapData);
        navigateToFragment(R.id.nav_controller_plant_scan_result, getActivity(), false, bundle);
    }

    private void gotoPlantDocFragment() {
        Bundle imageBundle = this.getArguments();
        if (imageBundle != null) {
            navigateToFragment(R.id.nav_controller_plant_doc_ask_question, getActivity(), false, imageBundle);
        }
    }

    private View.OnClickListener onButtonClickListener = v -> {
        //TODO: add Image to Bundle when navigating to Pflanzen-Doc
        gotoPlantDocFragment();
    };

    public void initData() {
        listDataHeader = new ArrayList<>();
        listHash = new HashMap<>();

        listDataHeader.add("Tipps");

        List<String> tipps = new ArrayList<>();
        tipps.add("Pflanzenfoto zur Pflanzenbestimmung hochladen: So gelingt es!");
        tipps.add("• Es kann nur ein Bild pro Scan upgeloaded und verarbeitet werden");
        tipps.add("• Bitte keine Fotos über 4MB Dateigröße hochladen. Größere Bilder bitte beschneiden oder kleinere Auflösung verwenden");
        tipps.add("• Bitte haben Sie etwas Geduld, bis die Ergebnisse sichtbar werden");

        listHash.put(listDataHeader.get(0), tipps);
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (resultCode == RESULT_OK && requestCode == IMAGE_CAMERA) {
            Bitmap bitmap = (Bitmap) data.getExtras().get("data");
            cameraBitmap = bitmap;
            imageView.setImageBitmap(bitmap);
            saveToInternalStorage(bitmap);
            showSaveToGalleryDialog();
            imageArrayList.add(new ItemImage(bitmap, 0));
            imageView.setVisibility(View.VISIBLE);
        } else if (resultCode == RESULT_OK && requestCode == IMAGE_ALBUM) {
            imageUri = data.getData();
            imageView.setImageURI(imageUri);

            Bitmap bitmap = convertUriToBitmap(getContext(), imageUri);

            saveToInternalStorage(bitmap);
            imageArrayList.add(new ItemImage(bitmap, 0));
            imageView.setVisibility(View.VISIBLE);
            GotoResultFragment();

        }
    }


    private void showSaveToGalleryDialog() {
        new GenericDialog.Builder(getContext())
                .setBitmapImage(cameraBitmap)
                .setMessageAppearance(R.color.text_all_riverBed, R.dimen.textSize_body_small)
                .addNewButton(R.style.PrimaryButtonStyle,
                        getResources().getString(R.string.all_photo) + " " + getResources().getString(R.string.all_save), R.dimen.textSize_body_medium, view -> {
                            requestForWritePermission();
                        })
                .addNewButton(R.style.SecondaryButtonStyle,
                        getResources().getString(R.string.all_scan), R.dimen.textSize_body_medium, view -> {
                            GotoResultFragment();
                        })
                .setButtonOrientation(LinearLayout.VERTICAL)
                .setCancelable(true)
                .generate();
    }

    private void requestForWritePermission() {
        if (ContextCompat.checkSelfPermission(getActivity(), Manifest.permission.WRITE_EXTERNAL_STORAGE) ==
                PackageManager.PERMISSION_GRANTED) {
            SaveImageToGallery saveImageToGallery = new SaveImageToGallery(getContext());
            saveImageToGallery.saveImage(cameraBitmap);
            GotoResultFragment();

        } else {
            requestPermissions(new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE},
                    EXTERNAL_STORAGE_WRITE_REQUEST_CODE);
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode,
                                           String permissions[], int[] grantResults) {
        switch (requestCode) {
            case EXTERNAL_STORAGE_WRITE_REQUEST_CODE: {
                if (isPermissionGranted(grantResults)) {
                    SaveImageToGallery saveImageToGallery = new SaveImageToGallery(getContext());
                    saveImageToGallery.saveImage(cameraBitmap);
                    GotoResultFragment();
                } else {
                    Toast.makeText(getActivity(), "Berechtigung verweigert!", Toast.LENGTH_SHORT).show();
                }
                return;
            }
        }
    }

    private boolean isPermissionGranted(int[] grantResults) {
        return grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED;
    }

    private String saveToInternalStorage(Bitmap bitmapImage) {
        String profile = "profile.jpg";
        int counter = 1;

        ContextWrapper cw = new ContextWrapper(getContext());
        // path to /data/data/yourapp/app_data/imageDir
        File directory = cw.getDir("imageDir", Context.MODE_PRIVATE);
        // Create imageDir

        File mypath = new File(directory, Integer.toString(counter) + profile);

        FileOutputStream fos = null;
        while (mypath.exists()) {
            ++counter;
            mypath = new File(directory, Integer.toString(counter) + profile);
        }
        try {
            fos = new FileOutputStream(mypath);
            // Use the compress method on the BitMap object to write image to the OutputStream
            bitmapImage.compress(Bitmap.CompressFormat.PNG, 100, fos);
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            try {
                fos.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
        return directory.getAbsolutePath();
    }

    private void loadImageFromStorage() {
        ContextWrapper cw = new ContextWrapper(getContext());
        // path to /data/data/yourapp/app_data/imageDir
        File directory = cw.getDir("imageDir", Context.MODE_PRIVATE);

        File listFile[] = directory.listFiles();

        for (File file : listFile) {


            String filePath = file.getPath();
            Bitmap bitmap = BitmapFactory.decodeFile(filePath);
            imageArrayList.add(new ItemImage(bitmap, 0));

        }
    }

    @Override
    public void itemClicked(ItemImage itemImage) {
        //Toast.makeText(getActivity(), "Item: " + itemImage.getImageView() + " clicked", Toast.LENGTH_SHORT).show();
        imageView.setImageBitmap(itemImage.getImageView());
        imageView.setVisibility(View.VISIBLE);
        GotoResultFragment();

    }

    @Override
    public void itemClicked(String itemText) {

    }

    @Override
    public void itemClicked(Section section) {
        //Toast.makeText(getActivity(), "Section: " + section.getName() + " clicked", Toast.LENGTH_SHORT).show();
    }

}