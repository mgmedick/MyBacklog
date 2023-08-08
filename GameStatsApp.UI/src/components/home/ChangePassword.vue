<template>
    <div class="mx-auto">
        <h2 class="text-center mb-3">Change Password</h2>
        <div class="mx-auto" style="max-width:400px;">
            <form v-if="islinkvalid" @submit.prevent="submitForm">         
                <div id="toastPlacement" ref="toastcontainer" class="toast-container position-fixed top-0 end-0" style="margin-top:70px;"> 
                    <div ref="errortoast" class="toast align-items-center text-white bg-danger border-0" role="alert" aria-live="assertive" aria-atomic="true">
                        <div class="d-flex">
                            <div class="toast-body">
                                <span class="msg-text"></span>
                            </div>
                            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                        </div>
                    </div>              
                    <div ref="successtoast" class="toast align-items-center text-white bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
                        <div class="d-flex">
                            <div class="toast-body">
                                <span class="msg-text"></span>
                            </div>
                            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                        </div>
                    </div>                         
                </div>
                <div class="mb-2">
                    <label for="txtPassword" class="form-label">New Password</label>
                    <input id="txtPassword" type="password" class="form-control" autocomplete="off" v-model.lazy="form.Password" @blur="v$.form.Password.$touch" aria-describedby="spnPasswordErrors">
                    <div>
                        <span id="spnPasswordErrors" class="form-text text-danger" v-for="error of v$.form.Password.$errors">{{ error.$message }}</span>
                    </div>
                </div>
                <div class="mb-3">
                    <label for="txtConfirmPassword" class="form-label">Confirm New Password</label>
                    <input id="txtConfirmPassword" type="password" class="form-control" autocomplete="off" v-model.lazy="form.ConfirmPassword" @blur="v$.form.ConfirmPassword.$touch" aria-describedby="spnConfirmPasswordErrors">
                    <div>
                        <span id="spnConfirmPasswordErrors" class="form-text text-danger" v-for="error of v$.form.ConfirmPassword.$errors">{{ error.$message }}</span>
                    </div>
                </div>
                <div class="row g-2 justify-content-center mb-3 mx-auto">
                    <button type="submit" class="btn btn-primary">Change Password</button>
                </div>            
            </form>
            <div v-else class="text-center">
                <div class="m-3">
                    <font-awesome-icon icon="fa-solid fa-hourglass-end" size="2xl" />
                </div>
                <div>
                    <span>Reset Password link has expired, please try again.</span>
                    <div>
                        <a href="/ResetPassword/">Reset Password</a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import { getFormData } from '../../js/common.js';
    import axios from 'axios';
    import useVuelidate from '@vuelidate/core';
    import { required, sameAs, helpers } from '@vuelidate/validators';
    import { Toast } from 'bootstrap';
    const { withAsync } = helpers;

    const passwordFormat = helpers.regex(/^(?=.*[A-Za-z])(?=.*\d)[._()-\/#&$@+\w\s]{8,30}$/)

    const asyncPasswordNotMatches = async (value) => {
        if (value === '') return true;

        return await axios.get('/Home/PasswordNotMatches', { params: { password: value } })
            .then(res => {
                return res.data;
            })
            .catch(err => { console.error(err); return Promise.reject(err); });
    }

    export default {
        name: "ChangePassword",
        setup() {
            return { v$: useVuelidate() }
        },
        props: {
            islinkvalid: Boolean
        },
        data() {
            return {
                form: {
                    Password: '',
                    ConfirmPassword: ''
                },
                showResetModal: false,
                showSuccess: false
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

                axios.post('/Home/ChangePassword', formData)
                    .then((res) => {
                        if (res.data.success) {
                            that.onSuccess("Password has been reset");
                        } else {
                            res.data.errorMessages.forEach(errorMsg => {
                                that.onError(errorMsg);                           
                            });     
                        }
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },
            onSuccess(successMsg) {
                var that = this;
                var el = that.$refs.successtoast.cloneNode(true);
                el.querySelector('.msg-text').innerHTML = successMsg;
                that.$refs.toastcontainer.appendChild(el);
                new Toast(el).show();    
            },
            onError(errorMsg) {
                var that = this;
                var el = that.$refs.errortoast.cloneNode(true);
                el.querySelector('.msg-text').innerHTML = errorMsg;
                that.$refs.toastcontainer.appendChild(el);
                new Toast(el).show();  
            }                                             
        },
        validations() {
            return {
                form: {
                    Password: {
                        required: helpers.withMessage('Password is required', required),
                        passwordFormat: helpers.withMessage('Must be between 8 - 30 characters with at least 1 number and letter', passwordFormat),
                        passwordNotMatches: helpers.withMessage('Password must differ from previous password', withAsync(asyncPasswordNotMatches))

                    },
                    ConfirmPassword: {
                        sameAsPassword: helpers.withMessage('Must match Password', sameAs(this.form.Password))
                    }
                }
            }
        }
    };
</script>


