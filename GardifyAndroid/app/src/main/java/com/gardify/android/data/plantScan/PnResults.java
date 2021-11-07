
package com.gardify.android.data.plantScan;

import java.util.List;
import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class PnResults {

    @SerializedName("results")
    @Expose
    private List<Result> results = null;
    @SerializedName("doubleResult")
    @Expose
    private List<DoubleResult> doubleResult = null;
    @SerializedName("InDb")
    @Expose
    private List<InDb> inDb = null;

    public List<Result> getResults() {
        return results;
    }

    public void setResults(List<Result> results) {
        this.results = results;
    }

    public List<DoubleResult> getDoubleResult() {
        return doubleResult;
    }

    public void setDoubleResult(List<DoubleResult> doubleResult) {
        this.doubleResult = doubleResult;
    }

    public List<InDb> getInDb() {
        return inDb;
    }

    public void setInDb(List<InDb> inDb) {
        this.inDb = inDb;
    }

}
