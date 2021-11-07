import { FormGroup } from '@angular/forms';

// custom validator to check that two fields match
 export function ZipCodeMustMatch(controlName: string, matchingControlName: any) {
    return (formGroup: FormGroup) => {
        const control = formGroup.controls[controlName];
        const matchingControl = formGroup.controls[matchingControlName];

        /* if (control.errors && !control.errors.zipCodeMustMatch) {
            // return if another validator has already found an error on the matchingControl
            return;
        } */
        const zipCode= Number(matchingControl.value)
        // set error on matchingControl if validation fails
        if (zipCode>=1000 && zipCode<=9999  ) {
            if(control.value!=="Schweiz" && control.value!=="Österreich"){
                matchingControl.setErrors({ zipCodeMustMatch: true });
            }else {
                    matchingControl.setErrors(null);
        }
        }
        if (zipCode>9999 && zipCode<=99999 ) {
            if(control.value==="Deutschland"){
            matchingControl.setErrors(null);
            }else {
                matchingControl.setErrors({ zipCodeMustMatch: true });
            }
        }
    }
}