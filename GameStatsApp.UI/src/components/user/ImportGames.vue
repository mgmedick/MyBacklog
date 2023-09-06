<template>
    <div class="mx-auto" style="max-width:400px;">
        <div class="mb-3">
            <font-awesome-icon icon="fa-solid fa-circle-info" class="text-secondary" size="xl" data-bs-toggle="tooltip" data-bs-title="Games will be imported into their associated list. Duplicate games will not be created. You can close this window at any time while importing."/>
        </div>
        <div class="row g-2 justify-content-center mb-3">      
            <button type="button" class="btn btn-outline-primary d-flex justify-content-center align-items-center" @click="onImportGamesClick($event, 1)" :disabled="importingUserAccountIDs[useraccounts.find(i => i.accountTypeID == 1)?.id]">
                <font-awesome-icon icon="fa-brands fa-steam" size="xl" style="color: #0a3169;"/>
                <div class="mx-auto">
                    <div class="align-self-start">
                        <span class="mx-auto useraccount-btn-text">{{ (importingUserAccountIDs[useraccounts.find(i => i.accountTypeID == 1)?.id] ? 'Importing ' : 'Import ') + 'Steam games' }}</span>
                    </div>
                    <div v-if="useraccounts.find(i => i.accountTypeID == 1)?.relativeImportLastRunDateString" class="align-self-end text-xs">
                        <span>{{ 'Last imported ' + useraccounts.find(i => i.accountTypeID == 1).relativeImportLastRunDateString }}</span>
                    </div>
                </div>
                <font-awesome-icon v-if="importingUserAccountIDs[useraccounts.find(i => i.accountTypeID == 1)?.id]" icon="fa-solid fa-spinner" spin size="xl"/>
            </button>     
            <button type="button" class="btn btn-outline-primary d-flex justify-content-center align-items-center" @click="onImportGamesClick($event, 2)" :disabled="importingUserAccountIDs[useraccounts.find(i => i.accountTypeID == 2)?.id]">
                <font-awesome-icon icon="fa-brands fa-xbox" size="xl" style="color: #107711;"/>
                <div class="mx-auto">
                    <div class="align-self-start">
                        <span class="mx-auto useraccount-btn-text">{{ (importingUserAccountIDs[useraccounts.find(i => i.accountTypeID == 2)?.id] ? 'Importing ' : 'Import ') + 'Xbox games' }}</span>
                    </div>
                    <div v-if="useraccounts.find(i => i.accountTypeID == 2)?.relativeImportLastRunDateString" class="align-self-end text-xs">
                        <span>{{ 'Last imported ' + useraccounts.find(i => i.accountTypeID == 2).relativeImportLastRunDateString }}</span>
                    </div>
                </div>
                <font-awesome-icon v-if="importingUserAccountIDs[useraccounts.find(i => i.accountTypeID == 2)?.id]" icon="fa-solid fa-spinner" spin size="xl"/>
            </button>
        </div>
    </div>
</template>
<script>
    import { successToast, errorToast } from '../../js/common.js';
    import { Tooltip } from 'bootstrap'; 
    import axios from 'axios';
    
    export default {
        name: "ImportGames",
        props: {
            useraccounts: Array,
            steamauthurl: String,
            microsoftauthurl: String,
            authsuccess: Boolean,
            authaccounttypeid: Number
        },
        data() {
            return {
                importingUserAccountIDs: JSON.parse(sessionStorage.getItem('importingUserAccountIDs')) ?? {}
            }
        },                                 
        watch: {
            importingUserAccountIDs: {
                handler(val, oldVal) {
                    sessionStorage.setItem('importingUserAccountIDs', JSON.stringify(val));
                    var isImporting = Object.keys(val).length > 0;
                    this.$emit('update:isimporting', isImporting);
                },
                deep: true
            }             
        },          
        created: function() {
        },         
        mounted: function () {
            var that = this;
            document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(el => {
                new Tooltip(el);
            });

            window.addEventListener('importingUserAccountIDsUpdate', (event) => {
                that.importingUserAccountIDs = JSON.parse(sessionStorage.getItem('importingUserAccountIDs')) ?? {};
            });

            if (that.authsuccess != null) {
                if (that.authsuccess) {
                    that.importGames(that.authaccounttypeid);
                } else {            
                    that.errorToast("Error authorizing account");       
                }
            }
        },        
        methods: {                    
            onImportGamesClick(e, accountTypeID, userAccount) {
                this.importGames(accountTypeID, userAccount)
            },
            importGames(accountTypeID) {
                var that = this;
                var userAccount = that.useraccounts.find(i => i.accountTypeID == accountTypeID);
                
                if (userAccount) {
                    if (!that.importingUserAccountIDs.hasOwnProperty(userAccount.id)) {
                        that.importingUserAccountIDs[userAccount.id] = { accountTypeName: userAccount.accountTypeName };
                    }

                    axios.post('/User/ImportGames', null,{ params: { userAccountID: userAccount?.id } }).then((res) => {
                        if (res.data.isAuthExpired) {
                            var redirectUrl = that.getRedirectUrl(accountTypeID);
                            location.href = redirectUrl;
                        } else {
                            if (res.data.success) {
                                //location.href = '/';
                                successToast("Imported " + userAccount.accountTypeName + " games");
                            } else {
                                errorToast("Error importing " + userAccount.accountTypeName + " games");                                               
                            }
                        }                                                                                                                           
                        delete that.importingUserAccountIDs[userAccount.id];
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
                } else {
                    var redirectUrl = that.getRedirectUrl(accountTypeID);
                    location.href = redirectUrl;                    
                }
            },
            getRedirectUrl(accountTypeID) {
                var result = '';

                switch (accountTypeID) {
                    case 1:
                        result = this.steamauthurl;
                        break;
                    case 2:
                        result = this.microsoftauthurl;
                        break;
                }

                return result;
            }                                                                                                                    
        }
    };
</script>


