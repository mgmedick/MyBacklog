<template>
    <div class="mx-auto">
        <h2 class="text-center mb-3">Welcome to GameStatsApp</h2>
        <div class="mx-auto" style="max-width:400px;">
            <form @submit.prevent="submitForm">
                <div class="toast-container position-absolute sticky-top p-3 top-0 end-0" id="toastPlacement" style="margin-top: 70px;"> 
                    <div ref="errortoasts" v-for="errorMessage in errorMessages" class="toast align-items-center text-white bg-danger border-0" role="alert" aria-live="assertive" aria-atomic="true">
                        <div class="d-flex">
                            <div class="toast-body">
                                <span>{{ errorMessage }}</span>
                            </div>
                            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                        </div>
                    </div>   
                </div>  
                <div class="mb-3">
                    <label for="txtEmail" class="form-label">Email</label>
                    <input id="txtEmail" type="text" class="form-control" autocomplete="off" v-model.lazy="form.Email" @blur="v$.form.Email.$touch" aria-describedby="spnEmailErrors">
                    <div>
                        <small id="spnEmailErrors" class="form-text text-danger" v-for="error of v$.form.Email.$errors">{{ error.$message }}</small>
                    </div>
                </div>
                <div class="row g-2 justify-content-center mb-3">
                    <button id="btnSignUp" type="submit" class="btn btn-primary">Sign Up</button>
                    <div class="text-center"><small class="fw-bold">OR</small></div>
                    <button type="submit" class="btn btn-primary">Continue with Facebook</button>
                    <div ref="googleLoginBtn"></div>
                </div>        
                <div>
                    <div v-if="loading">
                        <div class="d-flex m-3">
                            <div class="mx-auto">
                                <font-awesome-icon icon="fa-solid fa-spinner" spin size="lg" />
                            </div>
                        </div>
                    </div>
                    <div v-else-if="showSuccess">
                        <div class="p-3 alert alert-light">
                            <div class="mx-auto">
                                <div><span>To Activate your account click the activation link in the email we just sent you.</span></div>
                                <br />
                                <div>
                                    <span>If your email has not arrived try these steps:</span>
                                    <ul class="pl-4">
                                        <li>Wait 30 mins</li>
                                        <li>Check your spam folder</li>
                                        <li>Try Sign Up again</li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </form>
        </div>
    </div>
</template>
<script>
    import { getFormData } from '../../js/common.js';
    import axios from 'axios';
    import useVuelidate from '@vuelidate/core';
    import { required, email, helpers } from '@vuelidate/validators';
    import { Toast } from 'bootstrap';
    const { withAsync } = helpers;

    const asyncEmailNotExists = async (value) => {
        if (value === '') return true;

        return await axios.get('/Home/EmailNotExists', { params: { email: value } })
            .then(res => {
                return res.data;
            })
            .catch(err => { console.error(err); return Promise.reject(err); });
    }

    export default {
        name: "SignUp",
        setup() {
            return { v$: useVuelidate() }
        },
        props: {
            gclientid: String
        },        
        data() {
            return {
                form: {
                    Email: ''
                },
                loading: false,
                showSuccess: false,
                errorMessages: [],
                width: document.documentElement.clientWidth
            }
        },
        computed: {
        },
        mounted: function () {
            window.google.accounts.id.initialize({
                client_id: this.gclientid,
                callback: this.handleCredentialResponse,
                auto_select: true
            });

            this.renderGoogleButton();

            window.addEventListener('resize', this.resizeButtons); 
        },     
        methods: {
            async submitForm() {
                const isValid = await this.v$.$validate()
                if (!isValid) return

                var that = this;
                var formData = getFormData(this.form);
                this.loading = true;

                axios.post('/Home/SignUp', formData)
                    .then((res) => {
                        if (res.data.success) {
                            that.showSuccess = res.data.success;
                        } else {
                            that.errorMessages = res.data.errorMessages;
                            that.$nextTick(function() {
                                that.$refs.errortoasts?.forEach(el => {
                                    new Toast(el).show();
                                });
                            });                             
                        }
                        
                        that.loading = false;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            async handleCredentialResponse(response) {
                this.loading = true;

                axios.post('/Home/LoginOrSignUpByGoogle', null,{ params: { token: response.credential } })
                    .then((res) => {
                        if (res.data.success) {
                            if (res.data.isnewuser) {
                                location.href = '/Home/Welcome';
                            } else {
                                location.href = '/';
                            }
                        } else {
                            that.errorMessages = res.data.errorMessages;
                            that.$nextTick(function() {
                                that.$refs.errortoasts?.forEach(el => {
                                    new Toast(el).show();
                                });
                            });                             
                        }

                        that.loading = false;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            resizeButtons() {
                var that = this;
                if (that.width != document.documentElement.clientWidth) {  
                    that.width = document.documentElement.clientWidth;         
                    that.renderGoogleButton();
                }                 
            },
            renderGoogleButton() {
                var btnwidth = document.getElementById("btnSignUp").clientWidth;   

                const options = {
                    type: 'standard',
                    shape: 'pill',
                    theme: 'outline',
                    text: 'signin_with',
                    size: 'large',
                    logo_alignment: 'left',
                    width: btnwidth
                }

                window.google.accounts.id.renderButton(this.$refs.googleLoginBtn, options);
            }
        },
        validations() {
            return {
                form: {
                    Email: {
                        required: helpers.withMessage('Email is required', required),
                        email: helpers.withMessage('Email not found', email),
                        emailNotExists: helpers.withMessage('Email already exists', withAsync(asyncEmailNotExists))
                    }
                }
            }
        }
    };
</script>


