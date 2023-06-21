<template>
    <div class="mx-auto">
        <h2 class="text-center mb-3">Welcome to GameStatsApp</h2>
        <div class="mx-auto" style="max-width:400px;">
            <form @submit.prevent="submitForm">
                <div class="toast-container position-absolute p-3 top-0 end-0" id="toastPlacement"> 
                    <div ref="errortoasts" v-for="errorMessage in errorMessages" class="toast align-items-center text-white bg-danger border-0" role="alert" aria-live="assertive" aria-atomic="true">
                        <div class="d-flex">
                            <div class="toast-body">
                                <span>{{ errorMessage }}</span>
                            </div>
                            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                        </div>
                    </div>  
                </div>
                <div class="mb-2">
                    <label for="txtEmail" class="form-label">Email</label>
                    <input id="txtEmail" type="text" class="form-control" autocomplete="off" v-model.lazy="form.Email" @blur="v$.form.Email.$touch" aria-describedby="spnEmailErrors">
                    <div>
                        <span id="spnEmailErrors" class="form-text text-danger" v-for="error of v$.form.Email.$errors">{{ error.$message }}</span>
                    </div>
                </div>
                <div class="mb-3">
                    <label for="txtPassword" class="form-label">Password</label>
                    <input id="txtPassword" type="password" class="form-control" autocomplete="off" v-model.lazy="form.Password" @blur="v$.form.Password.$touch" aria-describedby="spnPasswordErrors">
                    <div>
                        <span id="spnPasswordErrors" class="form-text text-danger" v-for="error of v$.form.Password.$errors">{{ error.$message }}</span>
                    </div>
                    <div>
                        <a :href="'/Home/ResetPassword?email=' + form.Email" class="link-dark small">Forgot your password?</a>
                    </div>
                </div>   
                <div class="row g-2 justify-content-center">
                    <button id="btnLogin" type="submit" class="btn btn-primary">Log In</button>
                    <div class="text-center"><small class="fw-bold">OR</small></div>
                    <button type="submit" class="btn btn-outline-dark d-flex"><font-awesome-icon icon="fa-brands fa-facebook" size="xl" /><span class="mx-auto">Continue with Facebook</span></button>
                    <div ref="googleLoginBtn"></div>
                </div>
            </form>
        </div>
    </div>
</template>
<script>
    import { getFormData, setCookie } from '../../js/common.js';
    import axios from 'axios';
    import useVuelidate from '@vuelidate/core';
    import { required, helpers } from '@vuelidate/validators';
    import { Toast } from 'bootstrap';
    const { withAsync } = helpers;

    const asyncActiveEmailExists = async (value) => {
        if (value === '') return true;

        return await axios.get('/Home/ActiveEmailExists', { params: { email: value } })
            .then(res => {
                return res.data;
            })
            .catch(err => { console.error(err); return Promise.reject(err); });
    }

    export default {
        name: "Login",
        setup() {
            return { v$: useVuelidate() }
        },
        props: {
            gclientid: String
        },           
        data() {
            return {
                form: {
                    Email: '',
                    Password: ''
                },
                errorMessages: [],
                errorToasts: [],
                width: document.documentElement.clientWidth
            }
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

                axios.post('/Home/Login', formData)
                    .then((res) => {
                        if (res.data.success) {
                            location.href = '/';
                        } else {
                            that.errorMessages = res.data.errorMessages;
                        }
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            async handleCredentialResponse(response) {
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
                var btnwidth = document.getElementById("btnLogin").clientWidth;   

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
                        activeEmailExists: helpers.withMessage('Email not found', withAsync(asyncActiveEmailExists))
                    },
                    Password: {
                        required: helpers.withMessage('Password is required', required)
                    }
                }
            }
        }
    };
</script>


