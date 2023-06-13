<template>
    <form ref="form" @submit.prevent="submitForm" autocomplete="off">
        <div class="toast-container position-absolute p-3 top-0 end-0" id="toastPlacement"> 
            <div ref="errortoasts" v-for="errorMessage in errorMessages" class="toast align-items-center text-white bg-danger border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        <span>{{ errorMessage }}</span>
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
            <div ref="successtoast" class="toast align-items-center text-white bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        <span>Successfully updated username</span>
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>                         
        </div>     
        <div class="row g-1 justify-content-center align-items-center">   
            <div class="col-auto">
                <font-awesome-icon icon="fa-solid fa-at" size="xl" />
            </div> 
            <div class="col-auto">
                <input v-if="editUsername" id="txtUserName" type="text" class="form-control fs-3" autocomplete="off" v-model.lazy="form.Username" @blur="onUserNameBlur" aria-describedby="spnUserNameErrors">
                <label v-else class="form-control fs-3 border-0 ps-0">{{ form.Username }}</label>
                <div>
                    <span id="spnUserNameErrors" class="form-text text-danger" v-for="error of v$.form.Username.$errors">{{ error.$message }}</span>
                </div>
            </div>
            <div class="col-auto">
                <font-awesome-icon icon="fa-solid fa-square-pen" size="lg" @click="editUsername = !editUsername"/>
            </div>             
        </div>
    </form>
</template>
<script>
    import { getFormData } from '../../js/common.js';
    import axios from 'axios';
    import useVuelidate from '@vuelidate/core';
    import { required, helpers, sameAs } from '@vuelidate/validators';
    import { Toast } from 'bootstrap';    
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
                errorMessages: [],
                successToast: {},
                showSignUpModal: false,
                editUsername: false
            }
        },
        computed: {
        },
        mounted: function () {
            this.successToast = new Toast(this.$refs.successtoast);
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

                axios.post('/Home/ChangeUsername', formData)
                    .then((res) => {
                        if (res.data.success) {
                            that.successToast.show();
                        } else {
                            that.errorMessages = res.data.errorMessages;
                            that.$nextTick(function() {
                                that.$refs.errortoasts?.forEach(el => {
                                    new Toast(el).show();
                                });
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


