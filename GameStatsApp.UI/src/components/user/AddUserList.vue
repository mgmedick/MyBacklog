<template>
    <form ref="form" @submit.prevent="submitForm" autocomplete="off">
        <div class="d-flex align-items-center">
            <input id="txtListName" type="text" class="form-control fs-5" autocomplete="off" v-model.lazy="form.ListName" @blur="v$.form.ListName.$touch"  aria-describedby="spnListNameErrors">
            <div>
                <span id="spnListNameErrors" class="form-text text-danger" v-for="error of v$.form.ListName.$errors">{{ error.$message }}</span>
            </div>
        </div>
        <div class="row g-2 justify-content-center mb-3 mx-auto">
            <button type="submit" class="btn btn-primary">Add list</button>
        </div>         
    </form>
</template>
<script>
    import { getFormData, successToast, errorToast } from '../../js/common.js';
    import axios from 'axios';
    import useVuelidate from '@vuelidate/core';
    import { required, helpers } from '@vuelidate/validators';
    
    const { withAsync } = helpers;

    const listNameFormat = helpers.regex(/^[._()-\/#&$@+\w\s]{3,30}$/)

    const asyncListNameNotExists = async (value) => {  
        if(value === '') { return true; };            

        return await axios.get('/Home/UserListNameNotExists', { params: { listName: value } })
            .then(res => {
                return Promise.resolve(res.data);
            })
            .catch(err => { console.error(err); return Promise.reject(err); });
    }

    export default {
        name: "AddUserList",
        emits: ["success", "error"],
        props: {
            listname: String
        },
        setup() {
            return { v$: useVuelidate() }
        },
        data() {
            return {
                form: {
                    ListName: this.listname
                }
            }
        },
        computed: {
        },
        mounted: function () {
        },
        methods: {
            async submitForm() {
                const isValid = await this.v$.$validate()
                if (!isValid) return

                var that = this;
                var formData = getFormData(this.form);
                this.loading = true;

                return axios.post('/Home/AddUserList', formData)
                    .then((res) => {
                        if (res.data.success) {
                            successToast("Successfully added list");                           
                        } else {
                            res.data.errorMessages.forEach(errorMsg => {
                                errorToast(errorMsg);                           
                            });                                
                        }
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },                  
        },
        validations() {
            return {
                form: {
                    Username: {
                        required: helpers.withMessage('List name is required', required),
                        listNameFormat: helpers.withMessage('Must be between 3 - 30 characters', listNameFormat),
                        listNameNotExists: helpers.withMessage('List name already exists', withAsync(asyncListNameNotExists))
                    },
                }
            }
        }
    };
</script>


