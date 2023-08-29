<template>
    <form ref="form" @submit.prevent="submitForm" autocomplete="off">
        <div class="d-flex align-items-center justify-content-center">
            <font-awesome-icon icon="fa-solid fa-at" size="xl" class="me-2"/>
            <input v-if="editUsername" id="txtUserName" type="text" class="form-control fs-5" autocomplete="off" v-model.lazy="form.Username" @blur="onUserNameBlur" aria-describedby="spnUserNameErrors">
            <label v-else class="form-control fs-5 border-0 ps-0 w-auto">{{ form.Username }}</label>
            <font-awesome-icon icon="fa-solid fa-square-pen" size="lg" @click="editUsername = !editUsername"/>
            <div>
                <span id="spnUserNameErrors" class="form-text text-danger" v-for="error of v$.form.Username.$errors">{{ error.$message }}</span>
            </div>
        </div>
    </form>
</template>
<script>
    import { getFormData, successToast, errorToast } from '../../js/common.js';
    import axios from 'axios';
    import useVuelidate from '@vuelidate/core';
    import { required, helpers, sameAs } from '@vuelidate/validators';
    
    const { withAsync } = helpers;

    const usernameFormat = helpers.regex(/^[._()-\/#&$@+\w\s]{3,30}$/)

    const asyncUsernameNotExists = async (value) => {  
        if(value === '') { return true; };            

        return await axios.get('/Home/UsernameNotExists', { params: { username: value } })
            .then(res => {
                return Promise.resolve(res.data);
            })
            .catch(err => { console.error(err); return Promise.reject(err); });
    }

    export default {
        name: "ChangeUsername",
        emits: ["success", "error"],
        props: {
            username: String
        },
        setup() {
            return { v$: useVuelidate() }
        },
        data() {
            return {
                form: {
                    Username: this.username
                },
                editUsername: false
            }
        },
        computed: {
        },
        mounted: function () {
        },
        methods: {
            onUserNameBlur() {
                var that = this;
                this.v$.form.Username.$touch;  
                
                if (this.username != this.form.Username) {
                    this.submitForm().then(i => { that.editUsername = false });
                }
            },
            async submitForm() {
                const isValid = await this.v$.$validate()
                if (!isValid) return

                var that = this;
                var formData = getFormData(this.form);
                this.loading = true;

                return axios.post('/Home/ChangeUsername', formData)
                    .then((res) => {
                        if (res.data.success) {
                            successToast("Successfully updated username");                           
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
                        required: helpers.withMessage('Username is required', required),
                        usernameFormat: helpers.withMessage('Must be between 3 - 30 characters', usernameFormat),
                        usernameNotExists: helpers.withMessage('Username already exists', withAsync(asyncUsernameNotExists))
                    },
                }
            }
        }
    };
</script>


