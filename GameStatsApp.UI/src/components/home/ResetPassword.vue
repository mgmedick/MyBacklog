<template>
    <div class="mx-auto">
        <h2 class="text-center mb-3">Reset Password</h2>
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
                <div class="row g-2 justify-content-center mb-3 mx-auto">
                    <button type="submit" class="btn btn-primary">Send Email</button>
                </div>
                <div>
                    <div v-if="loading">
                        <div class="d-flex m-3">
                            <div class="mx-auto">
                                <font-awesome-icon icon="fa-solid fa-spinner" spin size="2xl" />
                            </div>
                        </div>
                    </div>
                    <div v-else-if="showSuccess">
                        <div class="p-3 alert alert-light">
                            <div class="mx-auto">
                                <div><span>To Reset your password click the link in the email we just sent you.</span></div>
                                <br />
                                <div>
                                    <span>If your email has not arrived try these steps:</span>
                                    <ul class="pl-4">
                                        <li>Wait 30 mins</li>
                                        <li>Check your spam folder</li>
                                        <li>Try Reset Password again</li>
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
        name: "ResetPassword",
        setup() {
            return { v$: useVuelidate() }
        },
        props: {
            email: String
        },        
        data() {
            return {
                form: {
                    Email: this.email
                },
                loading: false,
                showSuccess: false,
                errorMessages: [],
                errorToasts: []
            }
        },
        computed: {
        },
        created: function () {
        },
        methods: {
            async submitForm() {
                const isValid = await this.v$.$validate()
                if (!isValid) return

                var that = this;
                var formData = getFormData(this.form);
                this.loading = true;

                axios.post('/Home/ResetPassword', formData)
                    .then((res) => {
                        if (res.data.success){
                            that.showSuccess = true;
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
            }
        },
        validations() {
            return {
                form: {
                    Email: {
                        required: helpers.withMessage('Email is required', required),
                        activeEmailExists: helpers.withMessage('Email not found', withAsync(asyncActiveEmailExists))
                    }
                }
            }
        }
    };
</script>


